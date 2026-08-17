using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages;

public class DeneyimPaylasModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public DeneyimPaylasModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool Submitted { get; set; }

    public IList<Department> Departments { get; private set; } = new List<Department>();
    public IList<SubField> SubFields { get; private set; } = new List<SubField>();

    public class InputModel
    {
        [Required(ErrorMessage = "Adın gerekli.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Bölüm seçimi gerekli.")]
        public int DepartmentId { get; set; }

        public int? SubFieldId { get; set; }

        [Required(ErrorMessage = "Deneyimini yazman gerekli.")]
        [MinLength(20, ErrorMessage = "Deneyimini biraz daha detaylandırır mısın? (en az 20 karakter)")]
        public string Text { get; set; } = string.Empty;

        public string? Level { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadListsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var departmentExists = await _db.Departments.AnyAsync(d => d.Id == Input.DepartmentId);
        if (!departmentExists)
        {
            ModelState.AddModelError(string.Empty, "Seçilen bölüm bulunamadı.");
            return Page();
        }

        var review = new Review
        {
            Name = Input.Name,
            DepartmentId = Input.DepartmentId,
            SubFieldId = Input.SubFieldId,
            Text = Input.Text,
            Level = Input.Level,
            Status = ReviewStatus.Pending,
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        Submitted = true;
        return Page();
    }

    private async Task LoadListsAsync()
    {
        Departments = await _db.Departments.Where(d => d.IsActive).OrderBy(d => d.Name).ToListAsync();
        SubFields = await _db.SubFields.Include(sf => sf.Department).OrderBy(sf => sf.Name).ToListAsync();
    }
}
