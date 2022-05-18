using AmusmentPlanningSystem.Models;

namespace AmusmentPlanningSystem.Mocks
{
    public interface IGoogleService
    {
        public IEnumerable<int> GetDistanceMatrix(IEnumerable<Service> services, Models.Client client);
        public TimeSpan CalculateTravelTime(string addressStart, string addressFinish);
    }
    public class GoogleService : IGoogleService
    {
        public IEnumerable<int> GetDistanceMatrix(IEnumerable<Service> services, Models.Client client)
        {

            // These are mocks
            var rng = new Random();
            return Enumerable.Range(1, services.Count()).OrderBy(_ => rng.Next());
        }
        public TimeSpan CalculateTravelTime(string addressStart, string addressFinish)
        {
            return new TimeSpan(0, 15, 0);
        }
    }
}
