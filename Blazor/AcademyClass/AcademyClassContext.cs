using Microsoft.EntityFrameworkCore;

public class AcademyClassContext(DbContextOptions<AcademyClassContext> options) : DbContext(options)
{
    public DbSet<AcademyClass.Models.Discipline> Disciplines { get; set; } = default!;
}
