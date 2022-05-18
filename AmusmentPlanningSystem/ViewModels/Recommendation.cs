using AmusmentPlanningSystem.Models;

namespace AmusmentPlanningSystem.ViewModels
{
    public class Recommendation
    {
        public Category CurrentCategory { get; set; }
        public Category CategoryToCreateServiceFor { get; set; }
        public Price PriceRecommendation { get; set; }
    }
}
