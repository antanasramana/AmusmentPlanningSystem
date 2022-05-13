namespace AmusmentPlanningSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Client Client { get; set; }
        public Service Service { get; set; }
    }
}
