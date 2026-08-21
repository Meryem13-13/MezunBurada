using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages;

public class KaynakModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public KaynakModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public Resource Resource { get; private set; } = null!;

    // The specific roadmap step this link was clicked from, if any — gives a real, contextual
    // "why this was recommended" instead of only a generic free/affiliate note.
    public RoadmapStep? SourceStep { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, int? stepId)
    {
        var resource = await _db.Resources.FindAsync(id);
        if (resource is null)
        {
            return NotFound();
        }

        Resource = resource;

        if (stepId.HasValue)
        {
            SourceStep = await _db.RoadmapSteps
                .FirstOrDefaultAsync(s => s.Id == stepId.Value && s.ResourceId == id);
        }

        return Page();
    }
}
