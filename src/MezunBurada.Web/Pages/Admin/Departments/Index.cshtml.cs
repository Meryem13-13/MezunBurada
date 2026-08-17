using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Departments;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<Department> Departments { get; private set; } = new List<Department>();

    public async Task OnGetAsync()
    {
        Departments = await _db.Departments
            .Include(d => d.Category)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var department = await _db.Departments.FindAsync(id);
        if (department is not null)
        {
            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
