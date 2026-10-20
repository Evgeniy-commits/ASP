using System.ComponentModel.DataAnnotations.Schema;
using Academy.Models;

namespace Academy.Models
{
    public partial class Direction
    {
       public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
