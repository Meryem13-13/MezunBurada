namespace MezunBurada.Web.Models;

// Level-assessment question, asked at the SubField level (Sayfa 4 — Seviye testi).
public class LevelQuestion
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionDifficulty Difficulty { get; set; }
    public int Points { get; set; }

    public int SubFieldId { get; set; }
    public SubField? SubField { get; set; }

    public int? SkillId { get; set; }
    public Skill? Skill { get; set; }

    public ICollection<LevelQuestionOption> Options { get; set; } = new List<LevelQuestionOption>();
}
