using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using RoadmapModel = MezunBurada.Web.Models.Roadmap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Roadmaps;

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
    public SelectList LevelOptions { get; } = new(Enum.GetValues<ProficiencyLevel>());

    public class InputModel
    {
        [Required(ErrorMessage = "Kariyer yolu seçmelisin.")]
        public int CareerPathId { get; set; }

        [Required(ErrorMessage = "Roadmap başlığı gerekli.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public ProficiencyLevel Level { get; set; } = ProficiencyLevel.Beginner;
    }

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var roadmap = new RoadmapModel
        {
            CareerPathId = Input.CareerPathId,
            Title = Input.Title,
            Level = Input.Level,
        };
        _db.Roadmaps.Add(roadmap);
        await _db.SaveChangesAsync();

        return RedirectToPage("Edit", new { id = roadmap.Id });
    }

    private async Task LoadOptionsAsync()
    {
        var careerPaths = await _db.CareerPaths.OrderBy(cp => cp.Name).ToListAsync();
        CareerPathOptions = new SelectList(careerPaths, nameof(CareerPath.Id), nameof(CareerPath.Name));
    }
}
