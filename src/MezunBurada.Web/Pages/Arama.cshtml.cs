using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages;

public class AramaModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public AramaModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public string? SearchTerm { get; private set; }
    public IList<Department> Departments { get; private set; } = new List<Department>();
    public IList<SubField> SubFields { get; private set; } = new List<SubField>();
    public IList<Resource> Resources { get; private set; } = new List<Resource>();

    public bool HasResults => Departments.Any() || SubFields.Any() || Resources.Any();

    public async Task OnGetAsync(string? q)
    {
        SearchTerm = q;

        if (string.IsNullOrWhiteSpace(q))
        {
            return;
        }

        Departments = await _db.Departments
            .Where(d => d.IsActive && (d.Name.Contains(q) || d.ShortDescription.Contains(q)))
            .OrderBy(d => d.Name)
            .ToListAsync();

        SubFields = await _db.SubFields
            .Include(sf => sf.Department)
            .Where(sf => sf.Name.Contains(q) || sf.Description.Contains(q))
            .OrderBy(sf => sf.Name)
            .ToListAsync();

        Resources = await _db.Resources
            .Where(r => r.Title.Contains(q))
            .OrderBy(r => r.Title)
            .ToListAsync();
    }
}
