using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Departments;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList CategoryOptions { get; private set; } = null!;
    public SelectList DegreeTypeOptions { get; private set; } = null!;
    public SelectList ExamTypeOptions { get; private set; } = null!;

    public class InputModel
    {
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
        public DegreeType DegreeType { get; set; } = DegreeType.Lisans;

        [Required]
        public ExamType ExamType { get; set; } = ExamType.Sayisal;

        [Range(1, 8, ErrorMessage = "1-8 arası bir sayı gir.")]
        public int EducationDurationYears { get; set; } = 4;

        public bool IsActive { get; set; } = true;

        public string? MainTopics { get; set; }
        public string? SuitableFor { get; set; }
        public string? ChallengingFor { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _db.Departments.AnyAsync(d => d.Slug == Input.Slug))
        {
            ModelState.AddModelError("Input.Slug", "Bu slug zaten kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        _db.Departments.Add(new Department
        {
            CategoryId = Input.CategoryId,
            Name = Input.Name,
            Slug = Input.Slug,
            ShortDescription = Input.ShortDescription,
            LongDescription = Input.LongDescription ?? string.Empty,
            DegreeType = Input.DegreeType,
            ExamType = Input.ExamType,
            EducationDurationYears = Input.EducationDurationYears,
            IsActive = Input.IsActive,
            MainTopics = Input.MainTopics ?? string.Empty,
            SuitableFor = Input.SuitableFor ?? string.Empty,
            ChallengingFor = Input.ChallengingFor ?? string.Empty,
        });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        CategoryOptions = new SelectList(categories, nameof(Category.Id), nameof(Category.Name));
        DegreeTypeOptions = new SelectList(Enum.GetValues<DegreeType>());
        ExamTypeOptions = new SelectList(Enum.GetValues<ExamType>());
    }
}
