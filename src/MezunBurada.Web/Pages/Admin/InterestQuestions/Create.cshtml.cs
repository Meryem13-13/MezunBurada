using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.InterestQuestions;

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

        [Required(ErrorMessage = "Soru metni gerekli.")]
        [StringLength(300)]
        public string Text { get; set; } = string.Empty;
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

        var question = new InterestQuestion { DepartmentId = Input.DepartmentId, Text = Input.Text };
        _db.InterestQuestions.Add(question);
        await _db.SaveChangesAsync();

        // Straight to Edit so seçenekleri hemen ekleyebilsin — bir soru seçeneksiz bir işe yaramaz.
        return RedirectToPage("Edit", new { id = question.Id });
    }

    private async Task LoadOptionsAsync()
    {
        var departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        DepartmentOptions = new SelectList(departments, nameof(Department.Id), nameof(Department.Name));
    }
}
