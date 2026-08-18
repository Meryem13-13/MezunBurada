using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Categories;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Kategori adı gerekli.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug gerekli.")]
        [StringLength(100)]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug sadece küçük harf, rakam ve tire içerebilir.")]
        public string Slug { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Icon { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _db.Categories.AnyAsync(c => c.Slug == Input.Slug))
        {
            ModelState.AddModelError("Input.Slug", "Bu slug zaten kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _db.Categories.Add(new Category { Name = Input.Name, Slug = Input.Slug, Icon = Input.Icon });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
