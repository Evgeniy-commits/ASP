using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]       //откл. автоинкремент в таблице
        public int CourseID { get; set; }
        public string? Title { get; set; }
        public int Credits { get; set; }

        //Nav Prop
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
