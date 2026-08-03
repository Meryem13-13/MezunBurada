namespace MezunBurada.Web.Models;

public class LevelQuestionOption
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    public int LevelQuestionId { get; set; }
    public LevelQuestion? LevelQuestion { get; set; }
}
