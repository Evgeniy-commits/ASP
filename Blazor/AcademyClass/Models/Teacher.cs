using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyClass.Models
{
    public class Teacher : Human
    {
        [Key]
        [Column("teacher_id", TypeName = "SMALLINT")]
        public int teacher_id { get; set; }

        [Column("work_since")]
        public DateOnly? WorkSince { get; set; }

        [Column("rate", TypeName = "smallmoney")]
        public decimal? Rate { get; set; }

        //[Required]
        //[ForeignKey(nameof(Group))]
        //public int group { get; set; }

        //Nav Prop
        //public Group? Group { get; set; }
    }
}
