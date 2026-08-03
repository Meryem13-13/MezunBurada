namespace MezunBurada.Web.Models;

// Schema-only for now — no page or payment integration wired to this yet (future paid feature).
public class MentorSession
{
    public int Id { get; set; }
    public MentorSessionStatus Status { get; set; } = MentorSessionStatus.Planned;
    public DateTime? ScheduledAt { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User? User { get; set; }

    public int MentorId { get; set; }
    public Mentor? Mentor { get; set; }
}
