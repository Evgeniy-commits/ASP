using System.ComponentModel.DataAnnotations;

namespace AcademyClass.Models
{
    public class Student : Human
    {
        [Key]
        public int stud_id { get; set; }

        [Required]
        public int group {  get; set; }
    }
}
