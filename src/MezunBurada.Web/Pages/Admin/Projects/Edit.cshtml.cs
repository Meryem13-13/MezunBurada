using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Projects;

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

        [Required(ErrorMessage = "Proje adı gerekli.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Difficulty { get; set; }
        public string? EstimatedDuration { get; set; }
        public string? Technologies { get; set; }
        public string? Steps { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = project.Id,
            CareerPathId = project.CareerPathId,
            Name = project.Name,
            Description = project.Description,
            Difficulty = project.Difficulty,
            EstimatedDuration = project.EstimatedDuration,
            Technologies = project.Technologies,
            Steps = project.Steps,
        };

        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var project = await _db.Projects.FindAsync(Input.Id);
        if (project is null)
        {
            return NotFound();
        }

        project.CareerPathId = Input.CareerPathId;
        project.Name = Input.Name;
        project.Description = Input.Description ?? string.Empty;
        project.Difficulty = Input.Difficulty ?? string.Empty;
        project.EstimatedDuration = Input.EstimatedDuration ?? string.Empty;
        project.Technologies = Input.Technologies ?? string.Empty;
        project.Steps = Input.Steps ?? string.Empty;

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var careerPaths = await _db.CareerPaths.OrderBy(cp => cp.Name).ToListAsync();
        CareerPathOptions = new SelectList(careerPaths, nameof(CareerPath.Id), nameof(CareerPath.Name));
    }
}
