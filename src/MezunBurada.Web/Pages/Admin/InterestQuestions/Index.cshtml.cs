using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.InterestQuestions;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<InterestQuestion> Questions { get; private set; } = new List<InterestQuestion>();

    public async Task OnGetAsync()
    {
        Questions = await _db.InterestQuestions
            .Include(q => q.Department)
            .Include(q => q.Options)
            .OrderBy(q => q.Department!.Name).ThenBy(q => q.Id)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var question = await _db.InterestQuestions.FindAsync(id);
        if (question is not null)
        {
            _db.InterestQuestions.Remove(question);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
