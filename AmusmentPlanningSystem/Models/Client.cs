#nullable disable
using System.ComponentModel.DataAnnotations;

namespace AmusmentPlanningSystem.Models
{
    public class Client : User
    {
        public bool IsBlocked { get; set; }
        public string Address { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Rating> Ratings { get; set; }
    }
}
