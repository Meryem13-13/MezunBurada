namespace MezunBurada.Web.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Grants access to /admin. Not settable through any UI — only via direct database access,
    // a deliberate choice so an account can't grant itself admin rights through the app.
    public bool IsAdmin { get; set; }

    // Only the hash is stored — the raw token lives solely in the emailed/logged reset link,
    // so a database leak alone can't be used to reset anyone's password.
    public string? PasswordResetTokenHash { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    public ICollection<MentorSession> MentorSessions { get; set; } = new List<MentorSession>();
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
