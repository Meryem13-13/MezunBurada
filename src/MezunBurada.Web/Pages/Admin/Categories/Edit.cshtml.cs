using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Categories;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

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

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        Input = new InputModel { Id = category.Id, Name = category.Name, Slug = category.Slug, Icon = category.Icon };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _db.Categories.AnyAsync(c => c.Slug == Input.Slug && c.Id != Input.Id))
        {
            ModelState.AddModelError("Input.Slug", "Bu slug zaten kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var category = await _db.Categories.FindAsync(Input.Id);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = Input.Name;
        category.Slug = Input.Slug;
        category.Icon = Input.Icon;
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
