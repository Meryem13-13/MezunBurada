using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Roadmap;

public class IndexModel : PageModel
{
    private const string SubFieldSessionKey = "ResultSubFieldId";
    private const string LevelSessionKey = "ResultLevel";
    private const string TallySessionKey = "InterestTally";

    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public bool HasContent { get; private set; }
    public string DepartmentName { get; private set; } = string.Empty;
    public string SubFieldName { get; private set; } = string.Empty;
    public string CareerPathName { get; private set; } = string.Empty;
    public string CareerPathDescription { get; private set; } = string.Empty;
    public int CareerMatchPercent { get; private set; } = 87;

    public IReadOnlyList<RoadmapStep> Steps { get; private set; } = new List<RoadmapStep>();
    public IReadOnlyList<MarketDemandSkill> MarketDemandSkills { get; private set; } = new List<MarketDemandSkill>();
    public IReadOnlyList<CareerPathSkill> CvSkills { get; private set; } = new List<CareerPathSkill>();
    public Project? SuggestedProject { get; private set; }
    public string TotalDurationText { get; private set; } = "-";

    private int? _persistedMatchPercent;

    public async Task OnGetAsync()
    {
        var subFieldId = HttpContext.Session.GetInt32(SubFieldSessionKey);
        ProficiencyLevel? persistedLevel = null;

        // No test taken this browser session — if the visitor is logged in, use their most
        // recent saved result instead of losing it just because the session is fresh.
        if (subFieldId is null && User.Identity?.IsAuthenticated == true)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var latestResult = await _db.TestResults
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CompletedAt)
                .FirstOrDefaultAsync();

            if (latestResult is not null)
            {
                subFieldId = latestResult.SubFieldId;
                persistedLevel = latestResult.Level;
                _persistedMatchPercent = latestResult.CareerMatchPercent;
            }
        }

        // Still nothing (anonymous, no test taken) — fall back to demoing the one fully
        // seeded example (Backend Developer) so the page still shows something real.
        if (subFieldId is null)
        {
            var demoSubField = await _db.SubFields.FirstOrDefaultAsync(sf => sf.Slug == "backend-bilgisayar-muhendisligi");
            subFieldId = demoSubField?.Id;
        }

        if (subFieldId is null)
        {
            HasContent = false;
            return;
        }

        var subField = await _db.SubFields
            .Include(sf => sf.Department)
            .FirstOrDefaultAsync(sf => sf.Id == subFieldId);

        var careerPath = await _db.CareerPaths
            .Include(cp => cp.Roadmaps).ThenInclude(r => r.Steps).ThenInclude(s => s.Resource)
            .Include(cp => cp.Roadmaps).ThenInclude(r => r.Steps).ThenInclude(s => s.SuggestedProject)
            .Include(cp => cp.MarketDemandSkills)
            .Include(cp => cp.CareerPathSkills).ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(cp => cp.SubFieldId == subFieldId);

        DepartmentName = subField?.Department?.Name ?? string.Empty;
        SubFieldName = subField?.Name ?? string.Empty;

        if (subField is null || careerPath is null || careerPath.Roadmaps.Count == 0)
        {
            HasContent = false;
            return;
        }

        var levelRaw = HttpContext.Session.GetInt32(LevelSessionKey);
        var level = persistedLevel ?? (levelRaw.HasValue ? (ProficiencyLevel)levelRaw.Value : ProficiencyLevel.Beginner);
        var roadmap = careerPath.Roadmaps.FirstOrDefault(r => r.Level == level) ?? careerPath.Roadmaps.First();

        HasContent = true;
        CareerPathName = careerPath.Name;
        CareerPathDescription = careerPath.Description;
        Steps = roadmap.Steps.OrderBy(s => s.Order).ToList();
        MarketDemandSkills = careerPath.MarketDemandSkills.Take(5).ToList();
        CvSkills = careerPath.CareerPathSkills.ToList();
        SuggestedProject = Steps.Select(s => s.SuggestedProject).FirstOrDefault(p => p is not null);
        TotalDurationText = EstimateTotalDuration(Steps);
        CareerMatchPercent = _persistedMatchPercent ?? ComputeMatchPercent(subFieldId.Value);
    }

    // Step durations are free text ("1 hafta", "3 gün", ...) — best-effort sum of the leading
    // number treated as weeks (days counted as a fraction of a week); falls back to "-" if
    // nothing could be parsed rather than showing a made-up total.
    private static string EstimateTotalDuration(IEnumerable<RoadmapStep> steps)
    {
        double totalWeeks = 0;
        var parsedAny = false;

        foreach (var step in steps)
        {
            var match = Regex.Match(step.EstimatedDuration, @"(\d+)");
            if (!match.Success)
            {
                continue;
            }

            var value = double.Parse(match.Value);
            parsedAny = true;
            totalWeeks += step.EstimatedDuration.Contains("gün") ? value / 7.0 : value;
        }

        if (!parsedAny)
        {
            return "-";
        }

        return totalWeeks < 1
            ? "1 haftadan az"
            : $"~{Math.Round(totalWeeks)} hafta";
    }

    private int ComputeMatchPercent(int winningSubFieldId)
    {
        var json = HttpContext.Session.GetString(TallySessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return 87; // no interest-test tally in session (direct-nav demo) — keep the old sample value
        }

        var tally = JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
        var total = tally.Values.Sum();
        if (total == 0)
        {
            return 87;
        }

        return (int)Math.Round((double)tally.GetValueOrDefault(winningSubFieldId) / total * 100);
    }
}
