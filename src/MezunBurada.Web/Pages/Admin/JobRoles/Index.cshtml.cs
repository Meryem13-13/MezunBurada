using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.JobRoles;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<JobRole> JobRoles { get; private set; } = new List<JobRole>();

    public async Task OnGetAsync()
    {
        JobRoles = await _db.JobRoles
            .Include(j => j.CareerPath)
            .OrderBy(j => j.CareerPath!.Name).ThenBy(j => j.Title)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var jobRole = await _db.JobRoles.FindAsync(id);
        if (jobRole is not null)
        {
            _db.JobRoles.Remove(jobRole);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
