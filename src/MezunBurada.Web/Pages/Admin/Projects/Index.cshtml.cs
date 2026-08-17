using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Projects;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<Project> Projects { get; private set; } = new List<Project>();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Projects = await _db.Projects
            .Include(p => p.CareerPath)
            .OrderBy(p => p.CareerPath!.Name).ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is not null)
        {
            try
            {
                _db.Projects.Remove(project);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ErrorMessage = "Bu proje hâlâ bir roadmap adımında önerilen proje olarak kullanılıyor. Önce oradan kaldır.";
            }
        }

        return RedirectToPage();
    }
}
