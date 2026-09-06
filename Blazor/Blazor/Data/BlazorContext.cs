using Microsoft.EntityFrameworkCore;
using Blazor.Models;

namespace Blazor.Data
{
    public class BlazorContext : DbContext
    {
        public BlazorContext(DbContextOptions<BlazorContext> options) : base(options) { }

        public DbSet<Minesweeper> Minesweepers { get; set; } = null!;
    }
}
