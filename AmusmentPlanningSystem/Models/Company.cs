namespace AmusmentPlanningSystem.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Logo { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsClosed { get; set; }

        public int ServiceProviderId { get; set; }
        public ServiceProvider ServicesProvider { get; set; }
    }
}
