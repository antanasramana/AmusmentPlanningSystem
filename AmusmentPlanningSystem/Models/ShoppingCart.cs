namespace AmusmentPlanningSystem.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public List<Event> Events { get; set; }

    }
}
