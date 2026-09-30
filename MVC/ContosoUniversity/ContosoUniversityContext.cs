using Microsoft.EntityFrameworkCore;

public class ContosoUniversityContext(DbContextOptions<ContosoUniversityContext> options) : DbContext(options)
{
    public DbSet<ContosoUniversity.Models.Student> Students { get; set; } = default!;
    public DbSet<ContosoUniversity.Models.Course> Courses { get; set; } = default!;
    public DbSet<ContosoUniversity.Models.Enrollment> Enrollments { get; set; } = default!;
}
