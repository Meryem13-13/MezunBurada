using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MezunBurada.Web.Pages.Test;

public class SeviyeModel : PageModel
{
    public record AnswerOption(string Letter, string TextKey, bool IsSelected);

    public int CurrentStep { get; } = 3;
    public int TotalSteps { get; } = 3;
    public int CurrentQuestion { get; } = 5;
    public int TotalQuestions { get; } = 12;
    public int Difficulty { get; } = 2;
    public int MaxDifficulty { get; } = 5;

    public string DepartmentKey { get; } = "DeptComputerEngineering";
    public string SubFieldKey { get; } = "OptionAiTitle";

    public bool IsLastQuestion => CurrentQuestion >= TotalQuestions;

    public IReadOnlyList<AnswerOption> Options { get; } = new List<AnswerOption>
    {
        new("A", "OverfittingOptionA", false),
        new("B", "OverfittingOptionB", true),
        new("C", "OverfittingOptionC", false),
        new("D", "OverfittingOptionD", false),
    };

    public void OnGet()
    {
    }
}
