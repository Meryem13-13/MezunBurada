namespace MezunBurada.Web.Models;

public class Resource
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ResourceType Type { get; set; }

    public ICollection<RoadmapStep> RoadmapSteps { get; set; } = new List<RoadmapStep>();
}
