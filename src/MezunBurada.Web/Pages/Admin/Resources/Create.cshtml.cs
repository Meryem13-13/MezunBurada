using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MezunBurada.Web.Pages.Admin.Resources;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList TypeOptions { get; } = new(Enum.GetValues<ResourceType>());

    public class InputModel
    {
        [Required(ErrorMessage = "Başlık gerekli.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL gerekli.")]
        [Url(ErrorMessage = "Geçerli bir URL gir.")]
        public string Url { get; set; } = string.Empty;

        [Required]
        public ResourceType Type { get; set; } = ResourceType.Free;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _db.Resources.Add(new Resource { Title = Input.Title, Url = Input.Url, Type = Input.Type });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
