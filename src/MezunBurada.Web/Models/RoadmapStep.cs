namespace MezunBurada.Web.Models;

public class RoadmapStep
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Free text ("1 Hafta", "3 gün", "8+ hafta") — source content mixes units, not worth
    // normalizing to a single numeric unit for MVP.
    public string EstimatedDuration { get; set; } = string.Empty;

    public int RoadmapId { get; set; }
    public Roadmap? Roadmap { get; set; }

    // Self-referencing: the step that must be completed before this one (nullable — many steps have none).
    public int? PrerequisiteStepId { get; set; }
    public RoadmapStep? PrerequisiteStep { get; set; }

    public int? SkillId { get; set; }
    public Skill? Skill { get; set; }

    public int? ResourceId { get; set; }
    public Resource? Resource { get; set; }

    public int? SuggestedProjectId { get; set; }
    public Project? SuggestedProject { get; set; }
}
