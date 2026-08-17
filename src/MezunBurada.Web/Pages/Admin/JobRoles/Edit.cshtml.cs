using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.JobRoles;

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

        [Required(ErrorMessage = "Pozisyon adı gerekli.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        public string? RequiredSkills { get; set; }
        public string? Level { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var jobRole = await _db.JobRoles.FindAsync(id);
        if (jobRole is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = jobRole.Id,
            CareerPathId = jobRole.CareerPathId,
            Title = jobRole.Title,
            RequiredSkills = jobRole.RequiredSkills,
            Level = jobRole.Level,
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

        var jobRole = await _db.JobRoles.FindAsync(Input.Id);
        if (jobRole is null)
        {
            return NotFound();
        }

        jobRole.CareerPathId = Input.CareerPathId;
        jobRole.Title = Input.Title;
        jobRole.RequiredSkills = Input.RequiredSkills ?? string.Empty;
        jobRole.Level = Input.Level ?? string.Empty;

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var careerPaths = await _db.CareerPaths.OrderBy(cp => cp.Name).ToListAsync();
        CareerPathOptions = new SelectList(careerPaths, nameof(CareerPath.Id), nameof(CareerPath.Name));
    }
}
