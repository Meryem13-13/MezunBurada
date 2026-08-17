using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages;

public class DeneyimlerModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public DeneyimlerModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int? SelectedDepartmentId { get; private set; }
    public int? SelectedSubFieldId { get; private set; }
    public string? SearchTerm { get; private set; }

    public IList<Department> Departments { get; private set; } = new List<Department>();
    public IList<SubField> SubFields { get; private set; } = new List<SubField>();
    public IList<Review> Reviews { get; private set; } = new List<Review>();

    public int DepartmentCount { get; private set; }
    public int CareerPathCount { get; private set; }
    public int ExperienceCount { get; private set; }

    public bool HasActiveFilters => SelectedDepartmentId.HasValue || SelectedSubFieldId.HasValue || !string.IsNullOrWhiteSpace(SearchTerm);

    public async Task OnGetAsync(int? departmentId, int? subFieldId, string? q)
    {
        SelectedDepartmentId = departmentId;
        SelectedSubFieldId = subFieldId;
        SearchTerm = q;

        Departments = await _db.Departments.Where(d => d.IsActive).OrderBy(d => d.Name).ToListAsync();
        SubFields = await _db.SubFields.Include(sf => sf.Department).OrderBy(sf => sf.Name).ToListAsync();

        DepartmentCount = await _db.Departments.CountAsync(d => d.IsActive);
        CareerPathCount = await _db.CareerPaths.CountAsync();
        ExperienceCount = await _db.Reviews.CountAsync(r => r.Status == ReviewStatus.Approved);

        var query = _db.Reviews
            .Include(r => r.Department)
            .Include(r => r.SubField)
            .Where(r => r.Status == ReviewStatus.Approved)
            .AsQueryable();

        if (departmentId.HasValue)
        {
            query = query.Where(r => r.DepartmentId == departmentId);
        }

        if (subFieldId.HasValue)
        {
            query = query.Where(r => r.SubFieldId == subFieldId);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(r => r.Text.Contains(q) || r.Name.Contains(q));
        }

        Reviews = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }
}
