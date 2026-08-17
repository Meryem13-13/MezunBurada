using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.InterestQuestions;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public NewOptionModel NewOption { get; set; } = new();

    public SelectList DepartmentOptions { get; private set; } = null!;
    public SelectList SubFieldOptions { get; private set; } = null!;
    public List<InterestQuestionOption> Options { get; private set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bölüm seçmelisin.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Soru metni gerekli.")]
        [StringLength(300)]
        public string Text { get; set; } = string.Empty;
    }

    public class NewOptionModel
    {
        [StringLength(200)]
        public string? Text { get; set; }

        public int? MapsToSubFieldId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var question = await _db.InterestQuestions.FindAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        Input = new InputModel { Id = question.Id, DepartmentId = question.DepartmentId, Text = question.Text };
        await LoadPageDataAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadPageDataAsync(Input.Id);
            return Page();
        }

        var question = await _db.InterestQuestions.FindAsync(Input.Id);
        if (question is null)
        {
            return NotFound();
        }

        question.DepartmentId = Input.DepartmentId;
        question.Text = Input.Text;
        await _db.SaveChangesAsync();

        return RedirectToPage(new { id = Input.Id });
    }

    public async Task<IActionResult> OnPostAddOptionAsync(int id)
    {
        if (!string.IsNullOrWhiteSpace(NewOption.Text) && NewOption.MapsToSubFieldId.HasValue)
        {
            _db.InterestQuestionOptions.Add(new InterestQuestionOption
            {
                InterestQuestionId = id,
                Text = NewOption.Text,
                MapsToSubFieldId = NewOption.MapsToSubFieldId.Value,
            });
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteOptionAsync(int id, int optionId)
    {
        var option = await _db.InterestQuestionOptions.FindAsync(optionId);
        if (option is not null)
        {
            _db.InterestQuestionOptions.Remove(option);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    private async Task LoadPageDataAsync(int questionId)
    {
        var departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        DepartmentOptions = new SelectList(departments, nameof(Department.Id), nameof(Department.Name));

        // Only sub-fields under the question's own department make sense as an answer target.
        var subFields = await _db.SubFields
            .Where(sf => sf.DepartmentId == Input.DepartmentId)
            .OrderBy(sf => sf.Name)
            .ToListAsync();
        SubFieldOptions = new SelectList(subFields, nameof(SubField.Id), nameof(SubField.Name));

        Options = await _db.InterestQuestionOptions
            .Include(o => o.MapsToSubField)
            .Where(o => o.InterestQuestionId == questionId)
            .OrderBy(o => o.Id)
            .ToListAsync();
    }
}
