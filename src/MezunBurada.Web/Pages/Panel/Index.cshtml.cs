using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MezunBurada.Web.Pages.Panel;

public class IndexModel : PageModel
{
    public enum StepState { Done, Active, Upcoming }

    public record RoadmapStep(string TitleKey, StepState State);
    public record ActivityItem(string TextKey, string TimeKey);
    public record AchievementItem(string Icon, string LabelKey);

    public string UserName { get; } = "Meryem";
    public int RoadmapPercent { get; } = 32;
    public int StepsCompleted { get; } = 3;
    public int TotalSteps { get; } = 10;
    public int LearningHours { get; } = 12;
    public int Projects { get; } = 1;
    public int CareerMatchPercent { get; } = 87;

    public IReadOnlyList<RoadmapStep> Steps { get; } = new List<RoadmapStep>
    {
        new("StepTemelProgramlama", StepState.Done),
        new("StepGitGithub", StepState.Done),
        new("StepCSharpDotnet", StepState.Active),
        new("StepSql", StepState.Upcoming),
        new("StepRestApi", StepState.Upcoming),
        new("StepGercekProje", StepState.Upcoming),
    };

    public IReadOnlyList<ActivityItem> Activities { get; } = new List<ActivityItem>
    {
        new("ActivityGitHubDone", "TimeAgo2Days"),
        new("ActivityTemelProgramlamaDone", "TimeAgo5Days"),
        new("ActivityCSharpStarted", "TimeAgoToday"),
    };

    public IReadOnlyList<AchievementItem> Achievements { get; } = new List<AchievementItem>
    {
        new("🥇", "AchievementFirstStep"),
        new("🎯", "AchievementTestCompleted"),
        new("🔥", "AchievementStreak"),
    };

    public void OnGet()
    {
    }
}
