namespace MezunBurada.Web.Models;

// Represents a "Kariyer Alanı" / "Alt Dal" within a department (e.g. Backend, Frontend, AI/ML).
public class SubField
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;

    // Free-text, comma-separated — matches the source content template, no separate join tables.
    public string RequiredSkills { get; set; } = string.Empty;
    public string RecommendedTechnologies { get; set; } = string.Empty;
    public string ExamplePositions { get; set; } = string.Empty;
    public string StartingTopics { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<CareerPath> CareerPaths { get; set; } = new List<CareerPath>();
    public ICollection<LevelQuestion> LevelQuestions { get; set; } = new List<LevelQuestion>();
    public ICollection<InterestQuestionOption> InterestQuestionOptions { get; set; } = new List<InterestQuestionOption>();
}
