
using AmusmentPlanningSystem.Controllers.Client;
using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;

namespace AmusmentPlanningSystem.Controllers.ServiceProvider
{
    public class PriceChangerController
    {
        private readonly EventController _eventController;
        private readonly ServiceController _serviceController;
        private readonly ApplicationDbContext _context;

        public PriceChangerController(ApplicationDbContext context, EventController controller, ServiceController serviceController)
        {
            _eventController = controller;
            _context = context;
            _serviceController = serviceController;
        }

        public void CalculateNewPrices()
        {
            var startDate = DateTime.Now.Subtract(TimeSpan.FromDays(30));
            var eventsByTime = _eventController.GetAllEventsByTime(startDate, DateTime.Now);

            var groupedEvents = GroupEventsBasedOnService(eventsByTime);

            foreach(var keyVal in groupedEvents)
            {
                var service = keyVal.Value.First().Service;
                var events = keyVal.Value;
                
                if (service.ApplyDiscount > 0)
                {
                    var revenue = _serviceController.GetServiceRevenueOf30Days(service.Id);

                    if (!IsCompanyMoreProfitable(events, revenue))
                    {
                        // remove discount
                        var dbService = _context
                            .Service
                            !.SingleOrDefault(s => s.Id == service.Id);

                        // add 10% of discount
                        dbService!.ApplyDiscount = service.Price * 0.1;
                        _context.SaveChanges();
                    }
                } 
                else
                {
                    // remove discount
                    var dbService = _context
                        .Service
                        !.SingleOrDefault(s => s.Id == service.Id);
                    dbService!.ApplyDiscount = 0;
                    _context.SaveChanges();
                }
            }
        }

       
        private bool IsCompanyMoreProfitable(List<Event> events, double revenue)
        {
            double revenueOf5Days = events
                .Where(e => e.From.AddDays(5) >= DateTime.Now && e.From < DateTime.Now)
                .Select(e => e.Order.Sum)
                .Sum();

            double averagePerDay1 = revenueOf5Days / 5.0;
            double averagePerDay2 = revenue / 30.0;

            // if the company per 30 days earned less by 40% compared to the last 5 days average
            // then the company is not more profitable
            if (averagePerDay2 * 0.6 >= averagePerDay1)
            {
                return false;
            }

            return true;
        }
        private Dictionary<int, List<Event>> GroupEventsBasedOnService(List<Event> allEvents)
        {
            // Key is the id of the service
            // Value is the list of events it has
            var groupedEvents = new Dictionary<int, List<Event>>();
            foreach(var e in allEvents)
            {
                var service = e.Service;
                if (!groupedEvents.ContainsKey(service.Id))
                {
                    groupedEvents.Add(service.Id, new List<Event>());
                }

                groupedEvents[service.Id].Add(e);
            }

            return groupedEvents;
        }
    }
}
