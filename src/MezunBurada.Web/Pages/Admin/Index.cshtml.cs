using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int TotalUsers { get; private set; }
    public int TotalCompletedTests { get; private set; }
    public int PendingReviewsCount { get; private set; }
    public int TotalDepartments { get; private set; }

    public List<DepartmentBreakdownRow> DepartmentBreakdown { get; private set; } = new();
    public List<User> RecentUsers { get; private set; } = new();

    public class DepartmentBreakdownRow
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int SubFieldCount { get; set; }
        public int CareerPathCount { get; set; }
        public int CompletedTestCount { get; set; }
    }

    public async Task OnGetAsync()
    {
        TotalUsers = await _db.Users.CountAsync();
        TotalCompletedTests = await _db.TestResults.CountAsync();
        PendingReviewsCount = await _db.Reviews.CountAsync(r => r.Status == ReviewStatus.Pending);
        TotalDepartments = await _db.Departments.CountAsync();

        var departments = await _db.Departments
            .Include(d => d.SubFields).ThenInclude(sf => sf.CareerPaths)
            .OrderBy(d => d.Name)
            .ToListAsync();

        var testResultCountsBySubField = await _db.TestResults
            .GroupBy(t => t.SubFieldId)
            .Select(g => new { SubFieldId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.SubFieldId, g => g.Count);

        foreach (var dept in departments)
        {
            var completedForDept = dept.SubFields.Sum(sf => testResultCountsBySubField.GetValueOrDefault(sf.Id));
            DepartmentBreakdown.Add(new DepartmentBreakdownRow
            {
                DepartmentName = dept.Name,
                SubFieldCount = dept.SubFields.Count,
                CareerPathCount = dept.SubFields.Sum(sf => sf.CareerPaths.Count),
                CompletedTestCount = completedForDept,
            });
        }

        RecentUsers = await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .ToListAsync();
    }
}
