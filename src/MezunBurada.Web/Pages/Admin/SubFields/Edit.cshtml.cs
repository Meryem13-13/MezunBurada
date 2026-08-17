using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.SubFields;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList DepartmentOptions { get; private set; } = null!;

    public class InputModel
    {
        public int Id { get; set; }

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

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var subField = await _db.SubFields.FindAsync(id);
        if (subField is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = subField.Id,
            DepartmentId = subField.DepartmentId,
            Name = subField.Name,
            Slug = subField.Slug,
            Description = subField.Description,
            DifficultyLevel = subField.DifficultyLevel,
            RequiredSkills = subField.RequiredSkills,
            RecommendedTechnologies = subField.RecommendedTechnologies,
            ExamplePositions = subField.ExamplePositions,
            StartingTopics = subField.StartingTopics,
        };

        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _db.SubFields.AnyAsync(sf => sf.Slug == Input.Slug && sf.Id != Input.Id))
        {
            ModelState.AddModelError("Input.Slug", "Bu slug zaten kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var subField = await _db.SubFields.FindAsync(Input.Id);
        if (subField is null)
        {
            return NotFound();
        }

        subField.DepartmentId = Input.DepartmentId;
        subField.Name = Input.Name;
        subField.Slug = Input.Slug;
        subField.Description = Input.Description ?? string.Empty;
        subField.DifficultyLevel = Input.DifficultyLevel ?? string.Empty;
        subField.RequiredSkills = Input.RequiredSkills ?? string.Empty;
        subField.RecommendedTechnologies = Input.RecommendedTechnologies ?? string.Empty;
        subField.ExamplePositions = Input.ExamplePositions ?? string.Empty;
        subField.StartingTopics = Input.StartingTopics ?? string.Empty;

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        DepartmentOptions = new SelectList(departments, nameof(Department.Id), nameof(Department.Name));
    }
}
