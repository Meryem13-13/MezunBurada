using System.Text.Json;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Test;

public class IlgiAlaniModel : PageModel
{
    private const string DeptSessionKey = "TestDepartmentId";
    private const string AnsweredCountSessionKey = "InterestAnsweredCount";
    private const string TallySessionKey = "InterestTally";
    private const string ResultSubFieldSessionKey = "ResultSubFieldId";

    // Keys owned by Seviye/Roadmap — must also reset here so a previous department's leftover
    // level-test progress can't short-circuit a freshly-started one (SeviyeModel would otherwise
    // see a stale "already answered N questions" count from the last department and skip straight
    // to the roadmap with the wrong result).
    private const string LevelAnsweredCountSessionKey = "LevelAnsweredCount";
    private const string LevelPointsEarnedSessionKey = "LevelPointsEarned";
    private const string ResultLevelSessionKey = "ResultLevel";

    private readonly ApplicationDbContext _db;

    public IlgiAlaniModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int CurrentStep { get; } = 2;
    public int TotalSteps { get; } = 3;
    public int CurrentQuestion { get; private set; }
    public int TotalQuestions { get; private set; }
    public InterestQuestion Question { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? departmentId)
    {
        if (departmentId.HasValue)
        {
            // Starting fresh for this department — reset any earlier progress, including
            // leftover level-test state from a different department tested earlier this session.
            HttpContext.Session.SetInt32(DeptSessionKey, departmentId.Value);
            HttpContext.Session.SetInt32(AnsweredCountSessionKey, 0);
            HttpContext.Session.SetString(TallySessionKey, "{}");
            HttpContext.Session.Remove(ResultSubFieldSessionKey);
            HttpContext.Session.Remove(LevelAnsweredCountSessionKey);
            HttpContext.Session.Remove(LevelPointsEarnedSessionKey);
            HttpContext.Session.Remove(ResultLevelSessionKey);
        }

        var deptId = HttpContext.Session.GetInt32(DeptSessionKey);
        if (deptId is null)
        {
            return RedirectToPage("/Test/Bolum");
        }

        var questions = await _db.InterestQuestions
            .Where(q => q.DepartmentId == deptId)
            .Include(q => q.Options)
            .OrderBy(q => q.Id)
            .ToListAsync();

        if (questions.Count == 0)
        {
            return RedirectToPage("/Test/Bolum");
        }

        TotalQuestions = questions.Count;
        var answered = HttpContext.Session.GetInt32(AnsweredCountSessionKey) ?? 0;

        if (answered >= questions.Count)
        {
            var winningSubFieldId = GetTally().OrderByDescending(kv => kv.Value).First().Key;
            HttpContext.Session.SetInt32(ResultSubFieldSessionKey, winningSubFieldId);
            return RedirectToPage("/Test/Seviye");
        }

        CurrentQuestion = answered + 1;
        Question = questions[answered];
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int optionId)
    {
        var option = await _db.InterestQuestionOptions.FindAsync(optionId);
        if (option is null)
        {
            return RedirectToPage();
        }

        var tally = GetTally();
        tally[option.MapsToSubFieldId] = tally.GetValueOrDefault(option.MapsToSubFieldId) + 1;
        SetTally(tally);

        var answered = (HttpContext.Session.GetInt32(AnsweredCountSessionKey) ?? 0) + 1;
        HttpContext.Session.SetInt32(AnsweredCountSessionKey, answered);

        return RedirectToPage();
    }

    private Dictionary<int, int> GetTally()
    {
        var json = HttpContext.Session.GetString(TallySessionKey);
        return string.IsNullOrEmpty(json)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
    }

    private void SetTally(Dictionary<int, int> tally)
    {
        HttpContext.Session.SetString(TallySessionKey, JsonSerializer.Serialize(tally));
    }
}
