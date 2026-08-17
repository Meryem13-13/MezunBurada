using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.JobRoles;

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

        [Required(ErrorMessage = "Pozisyon adı gerekli.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        public string? RequiredSkills { get; set; }
        public string? Level { get; set; }
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

        _db.JobRoles.Add(new JobRole
        {
            CareerPathId = Input.CareerPathId,
            Title = Input.Title,
            RequiredSkills = Input.RequiredSkills ?? string.Empty,
            Level = Input.Level ?? string.Empty,
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
