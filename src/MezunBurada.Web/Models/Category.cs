namespace MezunBurada.Web.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Icon { get; set; }

    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
