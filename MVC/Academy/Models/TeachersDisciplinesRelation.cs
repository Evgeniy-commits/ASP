using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academy.Models
{
    [Table("TeachersDisciplinesRelation")]
    [PrimaryKey(nameof(Teacher), nameof(Discipline))]
    public partial class TeachersDisciplinesRelation
    {        
        [Column("teacher", TypeName = "SMALLINT")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Teacher { get; set; }
                
        [Column("discipline", TypeName = "SMALLINT")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Discipline { get; set; }

        [ForeignKey(nameof(Teacher))]
        [InverseProperty("TeacherDisciplines")]
        public virtual Teacher? TeacherNavigation { get; set; }

        [ForeignKey(nameof(Discipline))]
        [InverseProperty("TeacherDisciplines")]
        public virtual Discipline? DisciplineNavigation { get; set; }
    }
}