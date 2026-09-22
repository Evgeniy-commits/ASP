using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AcademyClass.Models;

public class Group
{
    [Key]
    public int group_id { get; set; }

    [Required]
    [StringLength(10, MinimumLength = 5)]
    [Column(TypeName = "NCHAR(10)")]
    public string group_name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "TINYINT")]
    [ForeignKey(nameof(Direction))]
    public int direction { get; set; } 

    [Column("weekdays", TypeName = "TINYINT")]
    public int? learning_days { get; set; }

    public TimeOnly? start_time { get; set; }

    public DateOnly? start_date { get; set; }


    //Navigation properties:
    public Direction Direction { get; set; } = default!;

   public virtual ICollection<Student> Students { get; set; } = default!;

}