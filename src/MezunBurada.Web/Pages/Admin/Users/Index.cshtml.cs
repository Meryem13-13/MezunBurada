using System.Security.Claims;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Users;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public string? SearchTerm { get; private set; }
    public int CurrentUserId { get; private set; }
    public List<UserRow> Users { get; private set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public class UserRow
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAdmin { get; set; }
        public int TestResultCount { get; set; }
    }

    public async Task OnGetAsync(string? q)
    {
        SearchTerm = q;
        CurrentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var query = _db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(u => u.FullName.Contains(q) || u.Email.Contains(q));
        }

        Users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserRow
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                IsAdmin = u.IsAdmin,
                TestResultCount = u.TestResults.Count,
            })
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostToggleAdminAsync(int id, string? q)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id == currentUserId)
        {
            ErrorMessage = "Kendi admin yetkini kendin kaldıramazsın — hesabının dışarıda kalmasını önlemek için bu engellendi.";
            return RedirectToPage(new { q });
        }

        var user = await _db.Users.FindAsync(id);
        if (user is not null)
        {
            user.IsAdmin = !user.IsAdmin;
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { q });
    }
}
