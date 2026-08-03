namespace MezunBurada.Web.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;

    // Free text — source content gives ranges ("1-2 hafta", "8+ hafta"), not single numbers.
    public string EstimatedDuration { get; set; } = string.Empty;

    // Free-text, comma-separated — matches the source content template.
    public string Technologies { get; set; } = string.Empty;
    public string Steps { get; set; } = string.Empty;

    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }

    public ICollection<RoadmapStep> SuggestedForSteps { get; set; } = new List<RoadmapStep>();
}
