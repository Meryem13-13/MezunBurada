using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MezunBurada.Web.Pages.Admin.Skills;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList CategoryOptions { get; } = new(Enum.GetValues<SkillCategory>());

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Beceri adı gerekli.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public SkillCategory Category { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var skill = await _db.Skills.FindAsync(id);
        if (skill is null)
        {
            return NotFound();
        }

        Input = new InputModel { Id = skill.Id, Name = skill.Name, Category = skill.Category };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var skill = await _db.Skills.FindAsync(Input.Id);
        if (skill is null)
        {
            return NotFound();
        }

        skill.Name = Input.Name;
        skill.Category = Input.Category;
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
