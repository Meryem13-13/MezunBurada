using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Skills;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<Skill> Skills { get; private set; } = new List<Skill>();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Skills = await _db.Skills.OrderBy(s => s.Category).ThenBy(s => s.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var skill = await _db.Skills.FindAsync(id);
        if (skill is not null)
        {
            try
            {
                _db.Skills.Remove(skill);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // FK is Restrict — some department/career path/question/roadmap step still uses this skill.
                ErrorMessage = "Bu beceri hâlâ bir bölüm, kariyer yolu, soru veya roadmap adımında kullanılıyor. Önce oradan kaldır.";
            }
        }

        return RedirectToPage();
    }
}
