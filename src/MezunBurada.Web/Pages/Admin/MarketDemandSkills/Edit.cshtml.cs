using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.MarketDemandSkills;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList CareerPathOptions { get; private set; } = null!;

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kariyer yolu seçmelisin.")]
        public int CareerPathId { get; set; }

        [Required(ErrorMessage = "Beceri adı gerekli.")]
        [StringLength(100)]
        public string SkillName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama gerekli.")]
        [StringLength(300)]
        public string DemandNote { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var skill = await _db.MarketDemandSkills.FindAsync(id);
        if (skill is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = skill.Id,
            CareerPathId = skill.CareerPathId,
            SkillName = skill.SkillName,
            DemandNote = skill.DemandNote,
        };

        await LoadCareerPathOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCareerPathOptionsAsync();
            return Page();
        }

        var skill = await _db.MarketDemandSkills.FindAsync(Input.Id);
        if (skill is null)
        {
            return NotFound();
        }

        skill.CareerPathId = Input.CareerPathId;
        skill.SkillName = Input.SkillName;
        skill.DemandNote = Input.DemandNote;
        skill.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadCareerPathOptionsAsync()
    {
        var careerPaths = await _db.CareerPaths.OrderBy(cp => cp.Name).ToListAsync();
        CareerPathOptions = new SelectList(careerPaths, nameof(CareerPath.Id), nameof(CareerPath.Name));
    }
}
