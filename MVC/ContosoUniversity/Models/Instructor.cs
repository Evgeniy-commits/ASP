using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Instructor
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Фамилия")]
        public string? LastName { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Имя")]
        public string? FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Дата трудоустройсва")]
        public DateTime HireDate { get; set; }

        //Calculated properties:
        [DisplayName("Инструктор")]
        public string FullName
        {
            get => $"{LastName} {FirstName}";
        }

        //Navigation properties:
        public ICollection<CourseAssignment>? CourseAssignments { get; set; }
        public OfficeAssignment? OfficeAssignment { get; set; }
    }
}
