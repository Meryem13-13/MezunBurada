using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MezunBurada.Web.Pages.Admin.Resources;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList TypeOptions { get; } = new(Enum.GetValues<ResourceType>());

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gerekli.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL gerekli.")]
        [Url(ErrorMessage = "Geçerli bir URL gir.")]
        public string Url { get; set; } = string.Empty;

        [Required]
        public ResourceType Type { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var resource = await _db.Resources.FindAsync(id);
        if (resource is null)
        {
            return NotFound();
        }

        Input = new InputModel { Id = resource.Id, Title = resource.Title, Url = resource.Url, Type = resource.Type };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var resource = await _db.Resources.FindAsync(Input.Id);
        if (resource is null)
        {
            return NotFound();
        }

        resource.Title = Input.Title;
        resource.Url = Input.Url;
        resource.Type = Input.Type;
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
