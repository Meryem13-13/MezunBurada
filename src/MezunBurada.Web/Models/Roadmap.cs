namespace MezunBurada.Web.Models;

public class Roadmap
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ProficiencyLevel Level { get; set; }

    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }

    public ICollection<RoadmapStep> Steps { get; set; } = new List<RoadmapStep>();
}
