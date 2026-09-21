using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyClass.Models
{
    public class Student : Human
    {
        [Key]
        public int stud_id { get; set; }

        [Required]
        [ForeignKey(nameof(Group))]
        public int group {  get; set; }

        //Nav Prop
        public Group? Group { get; set; }
    }
}
