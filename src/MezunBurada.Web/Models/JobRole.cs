namespace MezunBurada.Web.Models;

public class JobRole
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    // Free-text, comma-separated — matches the source content template.
    public string RequiredSkills { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;

    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }
}
