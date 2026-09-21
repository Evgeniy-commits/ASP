//#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyClass.Models
{
    public class Human
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression("^[A-ZА-Я][a-zа-я]+$")]
        public string? last_name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? first_name { get; set; }

        public string? middle_name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly? birth_date { get; set; }

        [EmailAddress]
        //[Required(AllowEmptyStrings = true)]
        public string? email { get; set; }

        [Phone]
        public string? phone { get; set; }
                
        [Column("photo", TypeName = "IMAGE")]
        public byte[]? photo { get; set; }

        //Calc properties
        public string FullName
        { get => $"{last_name} {first_name} {middle_name}"; }

        public int Age
        {
            get
            {
                if (birth_date is not null)
                {
                    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                    int age = today.Year - birth_date.Value.Year;

                    // Корректировка, если день рождения ещё не наступил в этом году
                    if (today < birth_date.Value.AddYears(age))
                        age--;

                    return age;
                }
                return 0;
            }
        }
    }
}
