namespace MezunBurada.Web.Models;

// Manually curated by admins — shown as "🔥 Piyasada Şu An Aranan" on the Yol Haritası page.
// UpdatedAt should be set to DateTime.UtcNow whenever an admin edits the row (not typed by hand),
// so the displayed date always reflects true recency.
public class MarketDemandSkill
{
    public int Id { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string DemandNote { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }
}
