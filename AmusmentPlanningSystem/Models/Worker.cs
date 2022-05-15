namespace AmusmentPlanningSystem.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime EditDate { get; set; }
        public DateTime CreationDate { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }

    }
}
