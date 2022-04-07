namespace AmusmentPlanningSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Service> Services { get; set; }
    }
}
