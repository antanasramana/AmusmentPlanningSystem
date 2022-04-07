namespace AmusmentPlanningSystem.Models
{
    public class Event
    {
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime Until { get; set; }
        public double Discount { get; set; }
        public Service Service { get; set; }
    }
}
