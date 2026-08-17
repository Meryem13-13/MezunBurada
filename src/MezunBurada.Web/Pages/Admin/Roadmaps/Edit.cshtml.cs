using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using RoadmapModel = MezunBurada.Web.Models.Roadmap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Roadmaps;

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
    public NewStepModel NewStep { get; set; } = new();

    public SelectList CareerPathOptions { get; private set; } = null!;
    public SelectList LevelOptions { get; } = new(Enum.GetValues<ProficiencyLevel>());
    public SelectList SkillOptions { get; private set; } = null!;
    public SelectList ResourceOptions { get; private set; } = null!;
    public SelectList ProjectOptions { get; private set; } = null!;
    public SelectList PrerequisiteStepOptions { get; private set; } = null!;
    public List<RoadmapStep> Steps { get; private set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kariyer yolu seçmelisin.")]
        public int CareerPathId { get; set; }

        [Required(ErrorMessage = "Roadmap başlığı gerekli.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public ProficiencyLevel Level { get; set; }
    }

    public class NewStepModel
    {
        [Range(1, 100)]
        public int Order { get; set; } = 1;

        [StringLength(200)]
        public string? Title { get; set; }

        public string? Description { get; set; }
        public string? EstimatedDuration { get; set; }
        public string? HowToApproach { get; set; }
        public int? SkillId { get; set; }
        public int? ResourceId { get; set; }
        public int? SuggestedProjectId { get; set; }
        public int? PrerequisiteStepId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var roadmap = await _db.Roadmaps.FindAsync(id);
        if (roadmap is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = roadmap.Id,
            CareerPathId = roadmap.CareerPathId,
            Title = roadmap.Title,
            Level = roadmap.Level,
        };

        var existingStepCount = await _db.RoadmapSteps.CountAsync(s => s.RoadmapId == id);
        NewStep.Order = existingStepCount + 1;

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

        var roadmap = await _db.Roadmaps.FindAsync(Input.Id);
        if (roadmap is null)
        {
            return NotFound();
        }

        roadmap.CareerPathId = Input.CareerPathId;
        roadmap.Title = Input.Title;
        roadmap.Level = Input.Level;
        await _db.SaveChangesAsync();

        return RedirectToPage(new { id = Input.Id });
    }

    public async Task<IActionResult> OnPostAddStepAsync(int id)
    {
        if (!string.IsNullOrWhiteSpace(NewStep.Title))
        {
            _db.RoadmapSteps.Add(new RoadmapStep
            {
                RoadmapId = id,
                Order = NewStep.Order,
                Title = NewStep.Title,
                Description = NewStep.Description ?? string.Empty,
                EstimatedDuration = NewStep.EstimatedDuration ?? string.Empty,
                HowToApproach = string.IsNullOrWhiteSpace(NewStep.HowToApproach) ? null : NewStep.HowToApproach,
                SkillId = NewStep.SkillId,
                ResourceId = NewStep.ResourceId,
                SuggestedProjectId = NewStep.SuggestedProjectId,
                PrerequisiteStepId = NewStep.PrerequisiteStepId,
            });
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteStepAsync(int id, int stepId)
    {
        var step = await _db.RoadmapSteps.FindAsync(stepId);
        if (step is not null)
        {
            _db.RoadmapSteps.Remove(step);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    private async Task LoadPageDataAsync(int roadmapId)
    {
        var careerPaths = await _db.CareerPaths.OrderBy(cp => cp.Name).ToListAsync();
        CareerPathOptions = new SelectList(careerPaths, nameof(CareerPath.Id), nameof(CareerPath.Name));

        var skills = await _db.Skills.OrderBy(s => s.Name).ToListAsync();
        SkillOptions = new SelectList(skills, nameof(Skill.Id), nameof(Skill.Name));

        var resources = await _db.Resources.OrderBy(r => r.Title).ToListAsync();
        ResourceOptions = new SelectList(resources, nameof(Resource.Id), nameof(Resource.Title));

        var projects = await _db.Projects.OrderBy(p => p.Name).ToListAsync();
        ProjectOptions = new SelectList(projects, nameof(Project.Id), nameof(Project.Name));

        Steps = await _db.RoadmapSteps
            .Include(s => s.Skill)
            .Include(s => s.Resource)
            .Include(s => s.SuggestedProject)
            .Include(s => s.PrerequisiteStep)
            .Where(s => s.RoadmapId == roadmapId)
            .OrderBy(s => s.Order)
            .ToListAsync();

        PrerequisiteStepOptions = new SelectList(Steps, nameof(RoadmapStep.Id), nameof(RoadmapStep.Title));
    }
}
