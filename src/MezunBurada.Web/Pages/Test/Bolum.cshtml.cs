using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Test;

public class BolumModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public BolumModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<Department> Departments { get; private set; } = new List<Department>();

    public async Task OnGetAsync()
    {
        Departments = await _db.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }
}
