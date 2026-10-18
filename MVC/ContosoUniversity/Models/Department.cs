using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ContosoUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "MONEY")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Дата запуска")]
        public DateTime StartDate { get; set; }

        public int? InstructorID { get; set; }

        //Navigation properties:
        public Instructor? Administrator { get; set; }
        public ICollection<Course>? Courses { get; set; }
    }
}
