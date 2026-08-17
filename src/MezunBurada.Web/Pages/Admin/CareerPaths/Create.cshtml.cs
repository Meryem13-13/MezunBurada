using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.CareerPaths;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList SubFieldOptions { get; private set; } = null!;

    public class InputModel
    {
        [Required(ErrorMessage = "Alt dal seçmelisin.")]
        public int SubFieldId { get; set; }

        [Required(ErrorMessage = "Kariyer yolu adı gerekli.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Difficulty { get; set; }
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

        _db.CareerPaths.Add(new CareerPath
        {
            SubFieldId = Input.SubFieldId,
            Name = Input.Name,
            Description = Input.Description ?? string.Empty,
            Difficulty = Input.Difficulty ?? string.Empty,
        });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var subFields = await _db.SubFields
            .Include(sf => sf.Department)
            .OrderBy(sf => sf.Department!.Name).ThenBy(sf => sf.Name)
            .ToListAsync();
        var items = subFields.Select(sf => new { sf.Id, Label = $"{sf.Department?.Name} · {sf.Name}" });
        SubFieldOptions = new SelectList(items, "Id", "Label");
    }
}
