#nullable disable
using System.ComponentModel.DataAnnotations;

namespace AmusmentPlanningSystem.Models
{
    public class Client
    {
        [Key]
        public int UserId { get; set; }
        public bool IsBlocked { get; set; }
        public string Address { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
