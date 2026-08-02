namespace MezunBurada.Web.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<SubField> SubFields { get; set; } = new List<SubField>();
}
