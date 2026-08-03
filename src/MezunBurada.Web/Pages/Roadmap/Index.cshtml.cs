using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MezunBurada.Web.Pages.Roadmap;

public class IndexModel : PageModel
{
    public record TimelineStep(string TitleKey, string DescKey, string TagKey, bool IsActive, string? HowToApproachKey = null);
    public record MarketDemandItem(string SkillName, string NoteKey);

    public string DepartmentKey { get; } = "DeptComputerEngineering";
    public int CareerMatchPercent { get; } = 87;

    public IReadOnlyList<TimelineStep> Steps { get; } = new List<TimelineStep>
    {
        new("RoadmapStep1Title", "RoadmapStep1Desc", "RoadmapStep1Tag", true),
        new("RoadmapStep2Title", "RoadmapStep2Desc", "RoadmapStep2Tag", false, "RoadmapStep2HowToApproach"),
        new("RoadmapStep3Title", "RoadmapStep3Desc", "RoadmapStep3Tag", false),
    };

    // Mirrors the MarketDemandSkill rows seeded for the Backend Developer career path —
    // kept as localized resx content (not read live from the DB) so the page stays
    // trilingual; the DB schema has no per-language columns yet.
    public IReadOnlyList<MarketDemandItem> MarketDemandSkills { get; } = new List<MarketDemandItem>
    {
        new("ASP.NET Core", "MarketSkillAspNetNote"),
        new("SQL", "MarketSkillSqlNote"),
        new("Docker", "MarketSkillDockerNote"),
        new("REST API", "MarketSkillRestApiNote"),
        new("Git", "MarketSkillGitNote"),
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
