namespace MezunBurada.Web.Models;

// Schema-only for now — no page or payment integration wired to this yet (future paid feature).
public class Mentor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;

    public int ExpertiseAreaId { get; set; }
    public CareerPath? ExpertiseArea { get; set; }

    public ICollection<MentorSession> Sessions { get; set; } = new List<MentorSession>();
}
