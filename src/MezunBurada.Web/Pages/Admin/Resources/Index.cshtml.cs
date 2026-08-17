using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Resources;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<Resource> Resources { get; private set; } = new List<Resource>();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Resources = await _db.Resources.OrderBy(r => r.Title).ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var resource = await _db.Resources.FindAsync(id);
        if (resource is not null)
        {
            try
            {
                _db.Resources.Remove(resource);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ErrorMessage = "Bu kaynak hâlâ bir roadmap adımında kullanılıyor. Önce oradan kaldır.";
            }
        }

        return RedirectToPage();
    }
}
