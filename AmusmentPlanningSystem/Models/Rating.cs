#nullable disable

namespace AmusmentPlanningSystem.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Evaluation { get; set; }
        public Service Service { get; set; }
        public Client Client { get; set; }
    }
}
