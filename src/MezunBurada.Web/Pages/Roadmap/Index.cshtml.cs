using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MezunBurada.Web.Pages.Roadmap;

public class IndexModel : PageModel
{
    public record TimelineStep(string TitleKey, string DescKey, string TagKey, bool IsActive);

    public string DepartmentKey { get; } = "DeptComputerEngineering";
    public int CareerMatchPercent { get; } = 87;

    public IReadOnlyList<TimelineStep> Steps { get; } = new List<TimelineStep>
    {
        new("RoadmapStep1Title", "RoadmapStep1Desc", "RoadmapStep1Tag", true),
        new("RoadmapStep2Title", "RoadmapStep2Desc", "RoadmapStep2Tag", false),
        new("RoadmapStep3Title", "RoadmapStep3Desc", "RoadmapStep3Tag", false),
    };

    public IReadOnlyList<string> TechTags { get; } = new List<string>
    {
        "ASP.NET Core", "Entity Framework", "PostgreSQL",
    };

    public IReadOnlyList<string> CvSkillKeys { get; } = new List<string>
    {
        "SkillRestApiArchitecture", "SkillSqlManagement", "SkillGitCicd",
        "SkillUnitTesting", "SkillAuthJwt", "SkillDockerBasics",
    };

    public void OnGet()
    {
    }
}
