using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Projects;

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

        [Required(ErrorMessage = "Proje adı gerekli.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Difficulty { get; set; }
        public string? EstimatedDuration { get; set; }
        public string? Technologies { get; set; }
        public string? Steps { get; set; }
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

        _db.Projects.Add(new Project
        {
            CareerPathId = Input.CareerPathId,
            Name = Input.Name,
            Description = Input.Description ?? string.Empty,
            Difficulty = Input.Difficulty ?? string.Empty,
            EstimatedDuration = Input.EstimatedDuration ?? string.Empty,
            Technologies = Input.Technologies ?? string.Empty,
            Steps = Input.Steps ?? string.Empty,
        });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var careerPaths = await _db.CareerPaths.OrderBy(cp => cp.Name).ToListAsync();
        CareerPathOptions = new SelectList(careerPaths, nameof(CareerPath.Id), nameof(CareerPath.Name));
    }
}
