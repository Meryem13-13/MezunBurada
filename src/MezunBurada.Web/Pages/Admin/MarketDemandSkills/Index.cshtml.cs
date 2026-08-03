using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.MarketDemandSkills;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<MarketDemandSkill> Skills { get; private set; } = new List<MarketDemandSkill>();

    public async Task OnGetAsync()
    {
        Skills = await _db.MarketDemandSkills
            .Include(m => m.CareerPath)
            .OrderBy(m => m.CareerPath!.Name)
            .ThenBy(m => m.SkillName)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var skill = await _db.MarketDemandSkills.FindAsync(id);
        if (skill is not null)
        {
            _db.MarketDemandSkills.Remove(skill);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
