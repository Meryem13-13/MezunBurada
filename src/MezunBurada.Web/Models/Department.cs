namespace MezunBurada.Web.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public DegreeType DegreeType { get; set; }
    public ExamType ExamType { get; set; }
    public int EducationDurationYears { get; set; }
    public bool IsActive { get; set; } = true;

    // Comma-separated short phrases (e.g. "Programlama, Veri Yapıları & Algoritmalar, ...").
    // Kept as free text rather than normalized child tables — MVP scope.
    public string MainTopics { get; set; } = string.Empty;
    public string SuitableFor { get; set; } = string.Empty;
    public string ChallengingFor { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<SubField> SubFields { get; set; } = new List<SubField>();
    public ICollection<InterestQuestion> InterestQuestions { get; set; } = new List<InterestQuestion>();
    public ICollection<DepartmentSkill> DepartmentSkills { get; set; } = new List<DepartmentSkill>();
    public ICollection<Faq> Faqs { get; set; } = new List<Faq>();
}
