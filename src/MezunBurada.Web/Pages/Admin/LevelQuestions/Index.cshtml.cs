using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.LevelQuestions;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<LevelQuestion> Questions { get; private set; } = new List<LevelQuestion>();

    public async Task OnGetAsync()
    {
        Questions = await _db.LevelQuestions
            .Include(q => q.SubField).ThenInclude(sf => sf!.Department)
            .Include(q => q.Options)
            .OrderBy(q => q.SubField!.Department!.Name).ThenBy(q => q.SubField!.Name).ThenBy(q => q.Id)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var question = await _db.LevelQuestions.FindAsync(id);
        if (question is not null)
        {
            _db.LevelQuestions.Remove(question);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
