namespace MezunBurada.Web.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SkillCategory Category { get; set; }

    public ICollection<DepartmentSkill> DepartmentSkills { get; set; } = new List<DepartmentSkill>();
    public ICollection<CareerPathSkill> CareerPathSkills { get; set; } = new List<CareerPathSkill>();
}
