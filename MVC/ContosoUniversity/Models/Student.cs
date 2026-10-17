using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("Фамилия")]
        public string? LastName { get; set; }

        [Required]
        [DisplayName("Имя")]
        public string? FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Дата поступления")]
        public DateTime EnrollmentDate { get; set; }

        //Calculated properties:
        [Display(Name = "Студент")]
        public string FullName
        {
            get => $"{LastName} {FirstName}";
        }

        //Nav Prop
        public ICollection<Enrollment>? Enrollments { get; set; } 
    }
}
