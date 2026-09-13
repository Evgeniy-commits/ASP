using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academy.Models
{
    public partial class Direction
    {
        [Key]
        [Column("direction_id", TypeName = "TINYINT")]
        public int DirectionId { get; set; }

        [Column("direction_name")]
        [StringLength(150)]
        public string? DirectionName { get; set; }
    }
}
