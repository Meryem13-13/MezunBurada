using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.CareerPaths;

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
    public NewSkillModel NewSkill { get; set; } = new();

    public SelectList SubFieldOptions { get; private set; } = null!;
    public SelectList SkillOptions { get; private set; } = null!;
    public SelectList LevelOptions { get; } = new(Enum.GetValues<ProficiencyLevel>());
    public List<CareerPathSkill> CareerPathSkills { get; private set; } = new();

    public class NewSkillModel
    {
        public int SkillId { get; set; }
        public ProficiencyLevel RequiredLevel { get; set; }
    }

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Alt dal seçmelisin.")]
        public int SubFieldId { get; set; }

        [Required(ErrorMessage = "Kariyer yolu adı gerekli.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Difficulty { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var careerPath = await _db.CareerPaths.FindAsync(id);
        if (careerPath is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = careerPath.Id,
            SubFieldId = careerPath.SubFieldId,
            Name = careerPath.Name,
            Description = careerPath.Description,
            Difficulty = careerPath.Difficulty,
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

        var careerPath = await _db.CareerPaths.FindAsync(Input.Id);
        if (careerPath is null)
        {
            return NotFound();
        }

        careerPath.SubFieldId = Input.SubFieldId;
        careerPath.Name = Input.Name;
        careerPath.Description = Input.Description ?? string.Empty;
        careerPath.Difficulty = Input.Difficulty ?? string.Empty;

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAddSkillAsync(int id)
    {
        var alreadyLinked = await _db.CareerPathSkills.AnyAsync(cps => cps.CareerPathId == id && cps.SkillId == NewSkill.SkillId);
        if (NewSkill.SkillId > 0 && !alreadyLinked)
        {
            _db.CareerPathSkills.Add(new CareerPathSkill
            {
                CareerPathId = id,
                SkillId = NewSkill.SkillId,
                RequiredLevel = NewSkill.RequiredLevel,
            });
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveSkillAsync(int id, int skillId)
    {
        var link = await _db.CareerPathSkills.FindAsync(id, skillId);
        if (link is not null)
        {
            _db.CareerPathSkills.Remove(link);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    private async Task LoadOptionsAsync()
    {
        var subFields = await _db.SubFields
            .Include(sf => sf.Department)
            .OrderBy(sf => sf.Department!.Name).ThenBy(sf => sf.Name)
            .ToListAsync();
        var items = subFields.Select(sf => new { sf.Id, Label = $"{sf.Department?.Name} · {sf.Name}" });
        SubFieldOptions = new SelectList(items, "Id", "Label");

        var skills = await _db.Skills.OrderBy(s => s.Name).ToListAsync();
        SkillOptions = new SelectList(skills, nameof(Skill.Id), nameof(Skill.Name));

        CareerPathSkills = await _db.CareerPathSkills
            .Include(cps => cps.Skill)
            .Where(cps => cps.CareerPathId == Input.Id)
            .ToListAsync();
    }
}
