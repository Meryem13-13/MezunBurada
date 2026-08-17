using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MezunBurada.Web.Pages.Admin.Skills;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList CategoryOptions { get; } = new(Enum.GetValues<SkillCategory>());

    public class InputModel
    {
        [Required(ErrorMessage = "Beceri adı gerekli.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public SkillCategory Category { get; set; } = SkillCategory.Teknik;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _db.Skills.Add(new Skill { Name = Input.Name, Category = Input.Category });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
