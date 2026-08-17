using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.LevelQuestions;

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

    public SelectList SubFieldOptions { get; private set; } = null!;
    public SelectList SkillOptions { get; private set; } = null!;
    public SelectList DifficultyOptions { get; } = new(Enum.GetValues<QuestionDifficulty>());
    public List<LevelQuestionOption> Options { get; private set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Alt dal seçmelisin.")]
        public int SubFieldId { get; set; }

        [Required(ErrorMessage = "Soru metni gerekli.")]
        [StringLength(300)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public QuestionDifficulty Difficulty { get; set; }

        [Range(1, 20, ErrorMessage = "1-20 arası bir puan gir.")]
        public int Points { get; set; }

        public int? SkillId { get; set; }
    }

    public class NewOptionModel
    {
        [StringLength(200)]
        public string? Text { get; set; }

        public bool IsCorrect { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var question = await _db.LevelQuestions.FindAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = question.Id,
            SubFieldId = question.SubFieldId,
            Text = question.Text,
            Difficulty = question.Difficulty,
            Points = question.Points,
            SkillId = question.SkillId,
        };

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

        var question = await _db.LevelQuestions.FindAsync(Input.Id);
        if (question is null)
        {
            return NotFound();
        }

        question.SubFieldId = Input.SubFieldId;
        question.Text = Input.Text;
        question.Difficulty = Input.Difficulty;
        question.Points = Input.Points;
        question.SkillId = Input.SkillId;
        await _db.SaveChangesAsync();

        return RedirectToPage(new { id = Input.Id });
    }

    public async Task<IActionResult> OnPostAddOptionAsync(int id)
    {
        if (!string.IsNullOrWhiteSpace(NewOption.Text))
        {
            _db.LevelQuestionOptions.Add(new LevelQuestionOption
            {
                LevelQuestionId = id,
                Text = NewOption.Text,
                IsCorrect = NewOption.IsCorrect,
            });
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteOptionAsync(int id, int optionId)
    {
        var option = await _db.LevelQuestionOptions.FindAsync(optionId);
        if (option is not null)
        {
            _db.LevelQuestionOptions.Remove(option);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    private async Task LoadPageDataAsync(int questionId)
    {
        var subFields = await _db.SubFields
            .Include(sf => sf.Department)
            .OrderBy(sf => sf.Department!.Name).ThenBy(sf => sf.Name)
            .ToListAsync();
        var items = subFields.Select(sf => new { sf.Id, Label = $"{sf.Department?.Name} · {sf.Name}" });
        SubFieldOptions = new SelectList(items, "Id", "Label");

        var skills = await _db.Skills.OrderBy(s => s.Name).ToListAsync();
        SkillOptions = new SelectList(skills, nameof(Skill.Id), nameof(Skill.Name));

        Options = await _db.LevelQuestionOptions
            .Where(o => o.LevelQuestionId == questionId)
            .OrderBy(o => o.Id)
            .ToListAsync();
    }
}
