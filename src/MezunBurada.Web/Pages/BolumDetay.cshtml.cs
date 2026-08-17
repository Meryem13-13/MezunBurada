using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages;

public class BolumDetayModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public BolumDetayModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public Department Department { get; private set; } = null!;
    public IList<SubField> SubFields { get; private set; } = new List<SubField>();
    public IList<Faq> Faqs { get; private set; } = new List<Faq>();
    public IList<Review> Reviews { get; private set; } = new List<Review>();

    // The enum identifiers can't carry Turkish diacritics (e.g. ExamType.Sayisal), so this
    // maps them back to correctly-spelled display text for this public-facing page.
    public string DegreeTypeDisplay => Department.DegreeType switch
    {
        DegreeType.OnLisans => "Ön Lisans",
        DegreeType.Lisans => "Lisans",
        _ => Department.DegreeType.ToString(),
    };

    public string ExamTypeDisplay => Department.ExamType switch
    {
        ExamType.Sayisal => "Sayısal",
        ExamType.EsitAgirlik => "Eşit Ağırlık",
        ExamType.Sozel => "Sözel",
        ExamType.Dil => "Dil",
        _ => Department.ExamType.ToString(),
    };

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var department = await _db.Departments
            .FirstOrDefaultAsync(d => d.Slug == slug && d.IsActive);

        if (department is null)
        {
            return NotFound();
        }

        Department = department;

        SubFields = await _db.SubFields
            .Where(sf => sf.DepartmentId == department.Id)
            .OrderBy(sf => sf.Name)
            .ToListAsync();

        Faqs = await _db.Faqs
            .Where(f => f.DepartmentId == department.Id)
            .ToListAsync();

        Reviews = await _db.Reviews
            .Include(r => r.SubField)
            .Where(r => r.DepartmentId == department.Id && r.Status == ReviewStatus.Approved)
            .OrderByDescending(r => r.CreatedAt)
            .Take(6)
            .ToListAsync();

        return Page();
    }
}
