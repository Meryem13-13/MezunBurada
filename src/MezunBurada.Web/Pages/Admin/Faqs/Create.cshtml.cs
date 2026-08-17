using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Faqs;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList DepartmentOptions { get; private set; } = null!;

    public class InputModel
    {
        [Required(ErrorMessage = "Bölüm seçmelisin.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Soru gerekli.")]
        [StringLength(300)]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cevap gerekli.")]
        [StringLength(2000)]
        public string Answer { get; set; } = string.Empty;
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

        _db.Faqs.Add(new Faq
        {
            DepartmentId = Input.DepartmentId,
            Question = Input.Question,
            Answer = Input.Answer,
        });
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        DepartmentOptions = new SelectList(departments, nameof(Department.Id), nameof(Department.Name));
    }
}
