namespace MezunBurada.Web.Models;

// Submitted from /iletisim (General) and /kurumlar-icin (Institutional) — one shared inbox,
// reviewed from Pages/Admin/Messages.
public class ContactMessage
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string Message { get; set; } = string.Empty;
    public ContactCategory Category { get; set; } = ContactCategory.General;
    public bool IsHandled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
