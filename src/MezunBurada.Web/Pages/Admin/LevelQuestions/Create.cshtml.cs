using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.LevelQuestions;

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
    public SelectList DifficultyOptions { get; } = new(Enum.GetValues<QuestionDifficulty>());

    public class InputModel
    {
        [Required(ErrorMessage = "Alt dal seçmelisin.")]
        public int SubFieldId { get; set; }

        [Required(ErrorMessage = "Soru metni gerekli.")]
        [StringLength(300)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Easy;

        [Range(1, 20, ErrorMessage = "1-20 arası bir puan gir.")]
        public int Points { get; set; } = 2;
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

        var question = new LevelQuestion
        {
            SubFieldId = Input.SubFieldId,
            Text = Input.Text,
            Difficulty = Input.Difficulty,
            Points = Input.Points,
        };
        _db.LevelQuestions.Add(question);
        await _db.SaveChangesAsync();

        return RedirectToPage("Edit", new { id = question.Id });
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
