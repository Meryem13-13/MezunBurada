using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Departments;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public int NewSkillId { get; set; }

    public SelectList CategoryOptions { get; private set; } = null!;
    public SelectList DegreeTypeOptions { get; private set; } = null!;
    public SelectList ExamTypeOptions { get; private set; } = null!;
    public SelectList SkillOptions { get; private set; } = null!;
    public List<DepartmentSkill> DepartmentSkills { get; private set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori seçmelisin.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Bölüm adı gerekli.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug gerekli.")]
        [StringLength(150)]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug sadece küçük harf, rakam ve tire içerebilir.")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kısa açıklama gerekli.")]
        [StringLength(300)]
        public string ShortDescription { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? LongDescription { get; set; }

        [Required]
        public DegreeType DegreeType { get; set; }

        [Required]
        public ExamType ExamType { get; set; }

        [Range(1, 8, ErrorMessage = "1-8 arası bir sayı gir.")]
        public int EducationDurationYears { get; set; }

        public bool IsActive { get; set; }

        public string? MainTopics { get; set; }
        public string? SuitableFor { get; set; }
        public string? ChallengingFor { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var department = await _db.Departments.FindAsync(id);
        if (department is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = department.Id,
            CategoryId = department.CategoryId,
            Name = department.Name,
            Slug = department.Slug,
            ShortDescription = department.ShortDescription,
            LongDescription = department.LongDescription,
            DegreeType = department.DegreeType,
            ExamType = department.ExamType,
            EducationDurationYears = department.EducationDurationYears,
            IsActive = department.IsActive,
            MainTopics = department.MainTopics,
            SuitableFor = department.SuitableFor,
            ChallengingFor = department.ChallengingFor,
        };

        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _db.Departments.AnyAsync(d => d.Slug == Input.Slug && d.Id != Input.Id))
        {
            ModelState.AddModelError("Input.Slug", "Bu slug zaten kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var department = await _db.Departments.FindAsync(Input.Id);
        if (department is null)
        {
            return NotFound();
        }

        department.CategoryId = Input.CategoryId;
        department.Name = Input.Name;
        department.Slug = Input.Slug;
        department.ShortDescription = Input.ShortDescription;
        department.LongDescription = Input.LongDescription ?? string.Empty;
        department.DegreeType = Input.DegreeType;
        department.ExamType = Input.ExamType;
        department.EducationDurationYears = Input.EducationDurationYears;
        department.IsActive = Input.IsActive;
        department.MainTopics = Input.MainTopics ?? string.Empty;
        department.SuitableFor = Input.SuitableFor ?? string.Empty;
        department.ChallengingFor = Input.ChallengingFor ?? string.Empty;

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAddSkillAsync(int id)
    {
        var alreadyLinked = await _db.DepartmentSkills.AnyAsync(ds => ds.DepartmentId == id && ds.SkillId == NewSkillId);
        if (NewSkillId > 0 && !alreadyLinked)
        {
            _db.DepartmentSkills.Add(new DepartmentSkill { DepartmentId = id, SkillId = NewSkillId });
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveSkillAsync(int id, int skillId)
    {
        var link = await _db.DepartmentSkills.FindAsync(id, skillId);
        if (link is not null)
        {
            _db.DepartmentSkills.Remove(link);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    private async Task LoadOptionsAsync()
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        CategoryOptions = new SelectList(categories, nameof(Category.Id), nameof(Category.Name));
        DegreeTypeOptions = new SelectList(Enum.GetValues<DegreeType>());
        ExamTypeOptions = new SelectList(Enum.GetValues<ExamType>());

        var skills = await _db.Skills.OrderBy(s => s.Name).ToListAsync();
        SkillOptions = new SelectList(skills, nameof(Skill.Id), nameof(Skill.Name));

        DepartmentSkills = await _db.DepartmentSkills
            .Include(ds => ds.Skill)
            .Where(ds => ds.DepartmentId == Input.Id)
            .ToListAsync();
    }
}
