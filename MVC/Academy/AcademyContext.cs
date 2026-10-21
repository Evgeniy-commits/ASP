using Academy.Models;
using Microsoft.EntityFrameworkCore;

public class AcademyContext(DbContextOptions<AcademyContext> options) : DbContext(options)
{
    public DbSet<Academy.Models.Student> Students { get; set; } = default!;
    public DbSet<Academy.Models.Teacher> Teachers { get; set; } = default!;
    public DbSet<Academy.Models.Group> Groups { get; set; } = default!;
    public DbSet<Academy.Models.Discipline> Disciplines { get; set; } = default!;
    public DbSet<Academy.Models.Direction> Directions { get; set; } = default!;

    public DbSet<TeachersDisciplinesRelation> TeachersDisciplinesRelations { get; set; }
    public DbSet<DisciplinesDirectionsRelation> DisciplinesDirectionsRelations { get; set; }
}
