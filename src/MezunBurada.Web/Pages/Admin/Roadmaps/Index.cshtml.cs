using MezunBurada.Web.Data;
using RoadmapModel = MezunBurada.Web.Models.Roadmap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Roadmaps;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<RoadmapModel> Roadmaps { get; private set; } = new List<RoadmapModel>();

    public async Task OnGetAsync()
    {
        Roadmaps = await _db.Roadmaps
            .Include(r => r.CareerPath)
            .Include(r => r.Steps)
            .OrderBy(r => r.CareerPath!.Name).ThenBy(r => r.Level)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var roadmap = await _db.Roadmaps.FindAsync(id);
        if (roadmap is not null)
        {
            _db.Roadmaps.Remove(roadmap);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
