using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Reviews;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<Review> PendingReviews { get; private set; } = new List<Review>();
    public IList<Review> ApprovedReviews { get; private set; } = new List<Review>();

    public async Task OnGetAsync()
    {
        var reviews = await _db.Reviews
            .Include(r => r.Department)
            .Include(r => r.SubField)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        PendingReviews = reviews.Where(r => r.Status == ReviewStatus.Pending).ToList();
        ApprovedReviews = reviews.Where(r => r.Status == ReviewStatus.Approved).ToList();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review is not null)
        {
            review.Status = ReviewStatus.Approved;
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review is not null)
        {
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
