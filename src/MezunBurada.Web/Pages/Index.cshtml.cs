using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<Department> Departments { get; private set; } = new List<Department>();
    public IList<Review> FeaturedReviews { get; private set; } = new List<Review>();

    public async Task OnGetAsync()
    {
        Departments = await _db.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Take(8)
            .ToListAsync();

        FeaturedReviews = await _db.Reviews
            .Include(r => r.Department)
            .Include(r => r.SubField)
            .Where(r => r.Status == ReviewStatus.Approved && r.IsFeatured)
            .OrderByDescending(r => r.CreatedAt)
            .Take(4)
            .ToListAsync();
    }
}
