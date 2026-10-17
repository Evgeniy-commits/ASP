using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class OfficeAssingnment
    {
        public int InstructorID { get; set; }

        [StringLength(50)]
        [DisplayName("Расположение офиса")]
        public string? Location { get; set; }


        //Navigation properties:
        public Instructor? Instructor { get; set; }
    }
}
