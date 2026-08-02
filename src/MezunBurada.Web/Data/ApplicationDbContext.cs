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
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<SubField> SubFields => Set<SubField>();
    public DbSet<Question> Questions => Set<Question>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Department>()
            .HasIndex(d => d.Slug)
            .IsUnique();

        modelBuilder.Entity<SubField>()
            .HasOne(sf => sf.Department)
            .WithMany(d => d.SubFields)
            .HasForeignKey(sf => sf.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.SubField)
            .WithMany(sf => sf.Questions)
            .HasForeignKey(q => q.SubFieldId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
