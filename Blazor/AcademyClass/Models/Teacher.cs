using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyClass.Models
{
    public class Teacher : Human
    {
        [Key]
        [Column("teacher_id", TypeName = "SMALLINT")]
        public int teacher_id { get; set; }

        [Column("work_since")]
        public DateOnly? WorkSince { get; set; }

        [Column("rate", TypeName = "smallmoney")]
        public decimal? Rate { get; set; }

        //Nav Prop
        public ICollection<TeachersDisciplinesRelation> DisciplinesRelations { get; set; } = default!;

        //Calc properties
        public record ExpInfo(int Years, int Months)
        {
            public string Display()
            {
                if (Years == 0 && Months == 0)
                    return "нет опыта";

                if (Years == 0)
                    return $"{Months} мес.";

                if (Months == 0)
                    return GetYearWord(Years);

                return $"{GetYearWord(Years)}";
            }

            private static string GetYearWord(int y)
            {
                if ((y % 100) >= 11 && (y % 100) <= 19)
                    return $"{y} лет";

                return (y % 10) 
                switch
                {
                    1 => $"{y} год",
                    2 or 3 or 4 => $"{y} года",
                    _ => $"{y} лет"
                };
            }
        }

        // Вычисляемое свойство. Не сохраняется в БД (атрибут NotMapped).
        [NotMapped]
        public ExpInfo Exp
        {
            get
            {
                if (WorkSince is not null)
                {
                    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                    DateOnly start = WorkSince.Value;

                    int months = (today.Year - start.Year) * 12 + (today.Month - start.Month);
                    if (today.Day < start.Day) months--;

                    int years = Math.Max(0, months / 12);
                    int month = Math.Max(0, months % 12);

                    return new ExpInfo(years, month);
                }
                return new ExpInfo(0, 0);
            }
        }
    }
}
