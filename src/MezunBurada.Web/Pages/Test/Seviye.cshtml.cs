using System.Security.Claims;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Test;

public class SeviyeModel : PageModel
{
    private const string SubFieldSessionKey = "ResultSubFieldId";
    private const string AnsweredCountSessionKey = "LevelAnsweredCount";
    private const string PointsEarnedSessionKey = "LevelPointsEarned";
    private const string ResultLevelSessionKey = "ResultLevel";

    private readonly ApplicationDbContext _db;

    public SeviyeModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int CurrentStep { get; } = 3;
    public int TotalSteps { get; } = 3;
    public int CurrentQuestion { get; private set; }
    public int TotalQuestions { get; private set; }
    public int MaxDifficulty { get; } = 5;
    public int Difficulty { get; private set; }

    public string DepartmentName { get; private set; } = string.Empty;
    public string SubFieldName { get; private set; } = string.Empty;
    public LevelQuestion Question { get; private set; } = null!;

    public bool IsLastQuestion => CurrentQuestion >= TotalQuestions;

    public async Task<IActionResult> OnGetAsync()
    {
        var subFieldId = HttpContext.Session.GetInt32(SubFieldSessionKey);
        if (subFieldId is null)
        {
            return RedirectToPage("/Test/Bolum");
        }

        var subField = await _db.SubFields
            .Include(sf => sf.Department)
            .FirstOrDefaultAsync(sf => sf.Id == subFieldId);
        if (subField is null)
        {
            return RedirectToPage("/Test/Bolum");
        }

        DepartmentName = subField.Department?.Name ?? string.Empty;
        SubFieldName = subField.Name;

        var questions = await _db.LevelQuestions
            .Where(q => q.SubFieldId == subFieldId)
            .Include(q => q.Options)
            .OrderBy(q => q.Id)
            .ToListAsync();

        if (questions.Count == 0)
        {
            // No level question bank yet for this career area — skip straight to the roadmap
            // with a default level rather than dead-ending the test.
            HttpContext.Session.SetInt32(ResultLevelSessionKey, (int)ProficiencyLevel.Beginner);
            await PersistIfAuthenticatedAsync();
            return RedirectToPage("/Roadmap/Index");
        }

        TotalQuestions = questions.Count;
        var answered = HttpContext.Session.GetInt32(AnsweredCountSessionKey) ?? 0;

        if (answered >= questions.Count)
        {
            var earned = HttpContext.Session.GetInt32(PointsEarnedSessionKey) ?? 0;
            var maxPoints = questions.Sum(q => q.Points);
            var ratio = maxPoints == 0 ? 0 : (double)earned / maxPoints;
            var level = ratio >= 0.75 ? ProficiencyLevel.Advanced
                : ratio >= 0.4 ? ProficiencyLevel.Intermediate
                : ProficiencyLevel.Beginner;
            HttpContext.Session.SetInt32(ResultLevelSessionKey, (int)level);
            await PersistIfAuthenticatedAsync();
            return RedirectToPage("/Roadmap/Index");
        }

        CurrentQuestion = answered + 1;
        Question = questions[answered];
        Difficulty = Question.Difficulty switch
        {
            QuestionDifficulty.Easy => 2,
            QuestionDifficulty.Medium => 3,
            QuestionDifficulty.Hard => 4,
            _ => 2,
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int optionId)
    {
        var option = await _db.LevelQuestionOptions
            .Include(o => o.LevelQuestion)
            .FirstOrDefaultAsync(o => o.Id == optionId);

        if (option is not null && option.IsCorrect)
        {
            var earned = (HttpContext.Session.GetInt32(PointsEarnedSessionKey) ?? 0) + option.LevelQuestion!.Points;
            HttpContext.Session.SetInt32(PointsEarnedSessionKey, earned);
        }

        var answered = (HttpContext.Session.GetInt32(AnsweredCountSessionKey) ?? 0) + 1;
        HttpContext.Session.SetInt32(AnsweredCountSessionKey, answered);

        return RedirectToPage();
    }

    // Already-signed-in users (as opposed to the register/login-time path in
    // SessionTestResultHelper's other two callers) get their result persisted the moment the
    // test finishes, rather than only by accident on some future login. The session keys are
    // cleared right after so a later login doesn't re-persist the same test as a duplicate row —
    // Roadmap's own "no session result, use latest persisted TestResult" fallback picks this same
    // result straight back up.
    private async Task PersistIfAuthenticatedAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await SessionTestResultHelper.PersistAsync(_db, HttpContext.Session, userId);
        HttpContext.Session.Remove(SubFieldSessionKey);
        HttpContext.Session.Remove(ResultLevelSessionKey);
        HttpContext.Session.Remove(AnsweredCountSessionKey);
        HttpContext.Session.Remove(PointsEarnedSessionKey);
    }
}
