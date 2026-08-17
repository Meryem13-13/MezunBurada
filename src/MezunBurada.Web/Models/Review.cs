namespace MezunBurada.Web.Models;

// A shared graduate experience shown on /deneyimler. Submitted via /deneyim-paylas as Pending,
// only shows publicly once an admin approves it (Pages/Admin/Reviews).
public class Review
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    // Self-reported by the person sharing, not derived from a TestResult — anonymous/unregistered
    // visitors can submit a review too, so there's no guaranteed real test result to pull from.
    public string? Level { get; set; }

    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int? SubFieldId { get; set; }
    public SubField? SubField { get; set; }
}
