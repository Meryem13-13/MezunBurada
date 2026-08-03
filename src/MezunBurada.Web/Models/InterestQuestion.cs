namespace MezunBurada.Web.Models;

// Interest-detection question, asked at the Department level (Sayfa 3 — İlgi Alanı testi).
public class InterestQuestion
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<InterestQuestionOption> Options { get; set; } = new List<InterestQuestionOption>();
}
