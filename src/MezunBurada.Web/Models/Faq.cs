namespace MezunBurada.Web.Models;

public class Faq
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
}
