namespace MezunBurada.Web.Models;

// A concrete job outcome nested under a SubField (e.g. "Backend Developer" under "Backend").
public class CareerPath
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;

    public int SubFieldId { get; set; }
    public SubField? SubField { get; set; }

    public ICollection<CareerPathSkill> CareerPathSkills { get; set; } = new List<CareerPathSkill>();
    public ICollection<Roadmap> Roadmaps { get; set; } = new List<Roadmap>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<JobRole> JobRoles { get; set; } = new List<JobRole>();
    public ICollection<MarketDemandSkill> MarketDemandSkills { get; set; } = new List<MarketDemandSkill>();
    public ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();
}
