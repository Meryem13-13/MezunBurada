using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.MarketDemandSkills;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList CareerPathOptions { get; private set; } = null!;

    public class InputModel
    {
        [Required(ErrorMessage = "Kariyer yolu seçmelisin.")]
        public int CareerPathId { get; set; }

        [Required(ErrorMessage = "Beceri adı gerekli.")]
        [StringLength(100)]
        public string SkillName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama gerekli.")]
        [StringLength(300)]
        public string DemandNote { get; set; } = string.Empty;
    }

    public async Task OnGetAsync()
    {
        await LoadCareerPathOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCareerPathOptionsAsync();
            return Page();
        }

        _db.MarketDemandSkills.Add(new MarketDemandSkill
        {
            CareerPathId = Input.CareerPathId,
            SkillName = Input.SkillName,
            DemandNote = Input.DemandNote,
            UpdatedAt = DateTime.UtcNow,
        });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadCareerPathOptionsAsync()
    {
        var careerPaths = await _db.CareerPaths.OrderBy(cp => cp.Name).ToListAsync();
        CareerPathOptions = new SelectList(careerPaths, nameof(CareerPath.Id), nameof(CareerPath.Name));
    }
}
