using Microsoft.EntityFrameworkCore;
using MezunBurada.Web.Models;

namespace MezunBurada.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<SubField> SubFields => Set<SubField>();
    public DbSet<CareerPath> CareerPaths => Set<CareerPath>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<DepartmentSkill> DepartmentSkills => Set<DepartmentSkill>();
    public DbSet<CareerPathSkill> CareerPathSkills => Set<CareerPathSkill>();
    public DbSet<InterestQuestion> InterestQuestions => Set<InterestQuestion>();
    public DbSet<InterestQuestionOption> InterestQuestionOptions => Set<InterestQuestionOption>();
    public DbSet<LevelQuestion> LevelQuestions => Set<LevelQuestion>();
    public DbSet<LevelQuestionOption> LevelQuestionOptions => Set<LevelQuestionOption>();
    public DbSet<Roadmap> Roadmaps => Set<Roadmap>();
    public DbSet<RoadmapStep> RoadmapSteps => Set<RoadmapStep>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<JobRole> JobRoles => Set<JobRole>();
    public DbSet<MarketDemandSkill> MarketDemandSkills => Set<MarketDemandSkill>();
    public DbSet<Mentor> Mentors => Set<Mentor>();
    public DbSet<MentorSession> MentorSessions => Set<MentorSession>();
    public DbSet<TestResult> TestResults => Set<TestResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        modelBuilder.Entity<Department>()
            .HasIndex(d => d.Slug)
            .IsUnique();

        modelBuilder.Entity<Department>()
            .HasOne(d => d.Category)
            .WithMany(c => c.Departments)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SubField>()
            .HasIndex(sf => sf.Slug)
            .IsUnique();

        modelBuilder.Entity<SubField>()
            .HasOne(sf => sf.Department)
            .WithMany(d => d.SubFields)
            .HasForeignKey(sf => sf.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CareerPath>()
            .HasOne(cp => cp.SubField)
            .WithMany(sf => sf.CareerPaths)
            .HasForeignKey(cp => cp.SubFieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // Department <-> Skill (many-to-many via DepartmentSkill)
        modelBuilder.Entity<DepartmentSkill>()
            .HasKey(ds => new { ds.DepartmentId, ds.SkillId });

        modelBuilder.Entity<DepartmentSkill>()
            .HasOne(ds => ds.Department)
            .WithMany(d => d.DepartmentSkills)
            .HasForeignKey(ds => ds.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DepartmentSkill>()
            .HasOne(ds => ds.Skill)
            .WithMany(s => s.DepartmentSkills)
            .HasForeignKey(ds => ds.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // CareerPath <-> Skill (many-to-many via CareerPathSkill)
        modelBuilder.Entity<CareerPathSkill>()
            .HasKey(cs => new { cs.CareerPathId, cs.SkillId });

        modelBuilder.Entity<CareerPathSkill>()
            .HasOne(cs => cs.CareerPath)
            .WithMany(cp => cp.CareerPathSkills)
            .HasForeignKey(cs => cs.CareerPathId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CareerPathSkill>()
            .HasOne(cs => cs.Skill)
            .WithMany(s => s.CareerPathSkills)
            .HasForeignKey(cs => cs.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // Interest test — Department-level question, options point to a SubField
        modelBuilder.Entity<InterestQuestion>()
            .HasOne(q => q.Department)
            .WithMany(d => d.InterestQuestions)
            .HasForeignKey(q => q.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterestQuestionOption>()
            .HasOne(o => o.InterestQuestion)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.InterestQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterestQuestionOption>()
            .HasOne(o => o.MapsToSubField)
            .WithMany(sf => sf.InterestQuestionOptions)
            .HasForeignKey(o => o.MapsToSubFieldId)
            .OnDelete(DeleteBehavior.Restrict);

        // Level test — SubField-level question with correct-answer options
        modelBuilder.Entity<LevelQuestion>()
            .HasOne(q => q.SubField)
            .WithMany(sf => sf.LevelQuestions)
            .HasForeignKey(q => q.SubFieldId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LevelQuestion>()
            .HasOne(q => q.Skill)
            .WithMany()
            .HasForeignKey(q => q.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LevelQuestionOption>()
            .HasOne(o => o.LevelQuestion)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.LevelQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Roadmap
        modelBuilder.Entity<Roadmap>()
            .HasOne(r => r.CareerPath)
            .WithMany(cp => cp.Roadmaps)
            .HasForeignKey(r => r.CareerPathId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoadmapStep>()
            .HasOne(s => s.Roadmap)
            .WithMany(r => r.Steps)
            .HasForeignKey(s => s.RoadmapId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoadmapStep>()
            .HasOne(s => s.PrerequisiteStep)
            .WithMany()
            .HasForeignKey(s => s.PrerequisiteStepId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoadmapStep>()
            .HasOne(s => s.Skill)
            .WithMany()
            .HasForeignKey(s => s.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoadmapStep>()
            .HasOne(s => s.Resource)
            .WithMany(res => res.RoadmapSteps)
            .HasForeignKey(s => s.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoadmapStep>()
            .HasOne(s => s.SuggestedProject)
            .WithMany(p => p.SuggestedForSteps)
            .HasForeignKey(s => s.SuggestedProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Project, JobRole — belong to a CareerPath
        modelBuilder.Entity<Project>()
            .HasOne(p => p.CareerPath)
            .WithMany(cp => cp.Projects)
            .HasForeignKey(p => p.CareerPathId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobRole>()
            .HasOne(j => j.CareerPath)
            .WithMany(cp => cp.JobRoles)
            .HasForeignKey(j => j.CareerPathId)
            .OnDelete(DeleteBehavior.Cascade);

        // FAQ — belongs to a Department
        modelBuilder.Entity<Faq>()
            .HasOne(f => f.Department)
            .WithMany(d => d.Faqs)
            .HasForeignKey(f => f.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Market demand — manually curated, belongs to a CareerPath
        modelBuilder.Entity<MarketDemandSkill>()
            .HasOne(m => m.CareerPath)
            .WithMany(cp => cp.MarketDemandSkills)
            .HasForeignKey(m => m.CareerPathId)
            .OnDelete(DeleteBehavior.Cascade);

        // Mentorship — schema only, not wired to any page/payment flow yet
        modelBuilder.Entity<Mentor>()
            .HasOne(m => m.ExpertiseArea)
            .WithMany(cp => cp.Mentors)
            .HasForeignKey(m => m.ExpertiseAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MentorSession>()
            .HasOne(s => s.User)
            .WithMany(u => u.MentorSessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MentorSession>()
            .HasOne(s => s.Mentor)
            .WithMany(m => m.Sessions)
            .HasForeignKey(s => s.MentorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Test results — persisted once tied to a real account
        modelBuilder.Entity<TestResult>()
            .HasOne(t => t.User)
            .WithMany(u => u.TestResults)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestResult>()
            .HasOne(t => t.SubField)
            .WithMany()
            .HasForeignKey(t => t.SubFieldId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestResult>()
            .HasOne(t => t.CareerPath)
            .WithMany()
            .HasForeignKey(t => t.CareerPathId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
