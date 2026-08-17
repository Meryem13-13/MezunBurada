using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Faqs;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<Faq> Faqs { get; private set; } = new List<Faq>();

    public async Task OnGetAsync()
    {
        Faqs = await _db.Faqs
            .Include(f => f.Department)
            .OrderBy(f => f.Department!.Name).ThenBy(f => f.Id)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var faq = await _db.Faqs.FindAsync(id);
        if (faq is not null)
        {
            _db.Faqs.Remove(faq);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
