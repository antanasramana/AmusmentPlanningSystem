namespace AmusmentPlanningSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public DateTime date { get; set; }
        public Payment? Payment { get; set; }
        public int? PaymentId { get; set; }
        public Client Client { get; set; }
        public int ClientId { get; set; }

    }
}
