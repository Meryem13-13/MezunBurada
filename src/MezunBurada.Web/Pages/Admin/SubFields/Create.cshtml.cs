using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.SubFields;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList DepartmentOptions { get; private set; } = null!;

    public class InputModel
    {
        [Required(ErrorMessage = "Bölüm seçmelisin.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Alt dal adı gerekli.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug gerekli.")]
        [StringLength(150)]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug sadece küçük harf, rakam ve tire içerebilir.")]
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? DifficultyLevel { get; set; }
        public string? RequiredSkills { get; set; }
        public string? RecommendedTechnologies { get; set; }
        public string? ExamplePositions { get; set; }
        public string? StartingTopics { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _db.SubFields.AnyAsync(sf => sf.Slug == Input.Slug))
        {
            ModelState.AddModelError("Input.Slug", "Bu slug zaten kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        _db.SubFields.Add(new SubField
        {
            DepartmentId = Input.DepartmentId,
            Name = Input.Name,
            Slug = Input.Slug,
            Description = Input.Description ?? string.Empty,
            DifficultyLevel = Input.DifficultyLevel ?? string.Empty,
            RequiredSkills = Input.RequiredSkills ?? string.Empty,
            RecommendedTechnologies = Input.RecommendedTechnologies ?? string.Empty,
            ExamplePositions = Input.ExamplePositions ?? string.Empty,
            StartingTopics = Input.StartingTopics ?? string.Empty,
        });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        DepartmentOptions = new SelectList(departments, nameof(Department.Id), nameof(Department.Name));
    }
}
