namespace MezunBurada.Web.Models;

// Join entity: skills broadly taught within a department.
public class DepartmentSkill
{
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int SkillId { get; set; }
    public Skill? Skill { get; set; }
}
