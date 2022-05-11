namespace AmusmentPlanningSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public DateTime EditDate { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Event> Events { get; set; }
        public ICollection<Rating> Ratings { get; set; }


    }
}
