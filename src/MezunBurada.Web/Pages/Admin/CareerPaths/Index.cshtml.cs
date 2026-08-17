using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.CareerPaths;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<CareerPath> CareerPaths { get; private set; } = new List<CareerPath>();

    public async Task OnGetAsync()
    {
        CareerPaths = await _db.CareerPaths
            .Include(cp => cp.SubField).ThenInclude(sf => sf!.Department)
            .OrderBy(cp => cp.SubField!.Department!.Name).ThenBy(cp => cp.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var careerPath = await _db.CareerPaths.FindAsync(id);
        if (careerPath is not null)
        {
            _db.CareerPaths.Remove(careerPath);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
