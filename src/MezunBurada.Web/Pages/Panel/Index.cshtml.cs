using System.Security.Claims;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Panel;

[Authorize]
public class IndexModel : PageModel
{
    public enum StepState { Done, Active, Upcoming }

    public record RoadmapStepView(string Title, StepState State);

    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public string UserName { get; private set; } = string.Empty;
    public bool HasTestResult { get; private set; }
    public bool HasRoadmapContent { get; private set; }

    public string DepartmentName { get; private set; } = string.Empty;
    public string SubFieldName { get; private set; } = string.Empty;
    public string LevelResourceKey { get; private set; } = "LevelBeginner";
    public int CareerMatchPercent { get; private set; }
    public string CareerPathName { get; private set; } = string.Empty;

    public IReadOnlyList<RoadmapStepView> Steps { get; private set; } = new List<RoadmapStepView>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId);
        if (user is null)
        {
            return RedirectToPage("/Giris");
        }

        UserName = user.FullName;

        var latestResult = await _db.TestResults
            .Where(t => t.UserId == userId)
            .Include(t => t.SubField).ThenInclude(sf => sf!.Department)
            .Include(t => t.CareerPath).ThenInclude(cp => cp!.Roadmaps).ThenInclude(r => r.Steps)
            .OrderByDescending(t => t.CompletedAt)
            .FirstOrDefaultAsync();

        if (latestResult is null)
        {
            HasTestResult = false;
            return Page();
        }

        HasTestResult = true;
        DepartmentName = latestResult.SubField?.Department?.Name ?? string.Empty;
        SubFieldName = latestResult.SubField?.Name ?? string.Empty;
        CareerMatchPercent = latestResult.CareerMatchPercent;
        LevelResourceKey = latestResult.Level switch
        {
            ProficiencyLevel.Intermediate => "LevelIntermediate",
            ProficiencyLevel.Advanced => "LevelAdvanced",
            _ => "LevelBeginner",
        };

        var roadmap = latestResult.CareerPath?.Roadmaps.FirstOrDefault(r => r.Level == latestResult.Level)
            ?? latestResult.CareerPath?.Roadmaps.FirstOrDefault();

        if (latestResult.CareerPath is null || roadmap is null)
        {
            HasRoadmapContent = false;
            return Page();
        }

        HasRoadmapContent = true;
        CareerPathName = latestResult.CareerPath.Name;
        Steps = roadmap.Steps
            .OrderBy(s => s.Order)
            .Select((s, i) => new RoadmapStepView(s.Title, i == 0 ? StepState.Active : StepState.Upcoming))
            .ToList();

        return Page();
    }
}
