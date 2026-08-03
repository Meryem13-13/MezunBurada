namespace MezunBurada.Web.Models;

// Join entity: skills required for a specific career path, at a required proficiency level.
public class CareerPathSkill
{
    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }

    public int SkillId { get; set; }
    public Skill? Skill { get; set; }

    public ProficiencyLevel RequiredLevel { get; set; }
}
