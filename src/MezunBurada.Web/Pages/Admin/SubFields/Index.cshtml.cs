using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.SubFields;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<SubField> SubFields { get; private set; } = new List<SubField>();

    public async Task OnGetAsync()
    {
        SubFields = await _db.SubFields
            .Include(sf => sf.Department)
            .OrderBy(sf => sf.Department!.Name).ThenBy(sf => sf.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var subField = await _db.SubFields.FindAsync(id);
        if (subField is not null)
        {
            _db.SubFields.Remove(subField);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
