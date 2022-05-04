using System.ComponentModel.DataAnnotations;

namespace AmusmentPlanningSystem.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public double Discount { get; set; }
        public Service Service { get; set; }
        public int ServiceId { get; set; }
        public Order? Order { get; set; }
        public int? OrderId { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }
        public int? ShoppingCartId { get; set; }
    }
}
