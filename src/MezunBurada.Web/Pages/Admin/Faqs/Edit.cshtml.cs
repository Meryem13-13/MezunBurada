using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Faqs;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList DepartmentOptions { get; private set; } = null!;

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bölüm seçmelisin.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Soru gerekli.")]
        [StringLength(300)]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cevap gerekli.")]
        [StringLength(2000)]
        public string Answer { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var faq = await _db.Faqs.FindAsync(id);
        if (faq is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = faq.Id,
            DepartmentId = faq.DepartmentId,
            Question = faq.Question,
            Answer = faq.Answer,
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

        var faq = await _db.Faqs.FindAsync(Input.Id);
        if (faq is null)
        {
            return NotFound();
        }

        faq.DepartmentId = Input.DepartmentId;
        faq.Question = Input.Question;
        faq.Answer = Input.Answer;

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        DepartmentOptions = new SelectList(departments, nameof(Department.Id), nameof(Department.Name));
    }
}
