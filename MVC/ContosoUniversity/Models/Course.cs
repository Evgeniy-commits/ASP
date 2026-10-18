using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]       //откл. автоинкремент в таблице
        public int CourseID { get; set; }
        public string? Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }

        //Nav Prop
        public ICollection<Enrollment>? Enrollments { get; set; }
        public Department? Department { get; set; }
        public ICollection<Instructor>? Instructors { get; set; }
        
    }
}
