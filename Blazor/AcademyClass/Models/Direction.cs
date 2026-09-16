#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyClass.Models
{
    public partial class Direction
    {
        [Key]
        [Column(TypeName = "TINYINT")]
        public int direction_id { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(50)")]
        [StringLength(50, MinimumLength = 2)]
        public string direction_name { get; set; }


        //Navigation Properties
        public ICollection<Group> Groups { get; set; }
    }
}
