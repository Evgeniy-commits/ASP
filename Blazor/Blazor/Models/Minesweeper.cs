using System.ComponentModel.DataAnnotations;

namespace Blazor.Models
{
    public class Minesweeper
    {
        //[Key]
        public int Id { get; set; }

        [Required]
        public string PlayerName { get; set; } = string.Empty;

        //[Required]
        public int Seconds { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
