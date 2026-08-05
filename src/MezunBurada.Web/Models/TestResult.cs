namespace MezunBurada.Web.Models;

// A completed İlgi Alanı + Seviye test run, persisted once a session's in-progress result
// gets tied to a real account (on register, or on login if the session still has one).
public class TestResult
{
    public int Id { get; set; }
    public ProficiencyLevel Level { get; set; }
    public int CareerMatchPercent { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User? User { get; set; }

    public int SubFieldId { get; set; }
    public SubField? SubField { get; set; }

    // Nullable — the resulting SubField might not have a CareerPath seeded yet.
    public int? CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }
}
