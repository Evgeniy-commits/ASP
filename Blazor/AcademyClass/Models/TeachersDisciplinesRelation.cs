using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AcademyClass.Models
{
    [PrimaryKey("teacher", "discipline")]
    public class TeachersDisciplinesRelation
    {
        [Column("teacher", TypeName = "SMALLINT")]
        [ForeignKey(nameof(Teacher))]
        public int teacher { get; set; }

        [ForeignKey(nameof(Discipline))]
        [Column("discipline", TypeName = "SMALLINT")]
        public int discipline { get; set; }

        //Nav Prop
        public Teacher? Teacher { get; set; }
        public Discipline? Discipline { get; set; }
    }
}
