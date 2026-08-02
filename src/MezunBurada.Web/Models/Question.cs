namespace MezunBurada.Web.Models;

public class Question
{
    public int Id { get; set; }
    public QuestionType QuestionType { get; set; }
    public string Text { get; set; } = string.Empty;
    public string OptionsJson { get; set; } = "[]";
    public int Difficulty { get; set; }

    public int SubFieldId { get; set; }
    public SubField? SubField { get; set; }
}
