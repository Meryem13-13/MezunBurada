namespace MezunBurada.Web.Models;

public class SubField
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
