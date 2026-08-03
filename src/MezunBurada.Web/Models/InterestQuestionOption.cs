namespace MezunBurada.Web.Models;

public class InterestQuestionOption
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public int InterestQuestionId { get; set; }
    public InterestQuestion? InterestQuestion { get; set; }

    // Which career area (SubField) this answer points toward.
    public int MapsToSubFieldId { get; set; }
    public SubField? MapsToSubField { get; set; }
}
