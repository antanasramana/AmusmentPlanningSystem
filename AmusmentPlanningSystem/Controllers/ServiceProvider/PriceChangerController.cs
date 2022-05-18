
using AmusmentPlanningSystem.Controllers.Client;
using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmusmentPlanningSystem.Controllers.ServiceProvider
{
    public class PriceChangerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PriceChangerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CalculateNewPrices()
        {
            var startDate = DateTime.Now.Subtract(TimeSpan.FromDays(30));
            var eventsByTime = _context
                .Events
                .Include(e => e.Order)
                .Include(e => e.Service)
                .Where(e => e.From >= startDate && e.To <= DateTime.Now)
                .ToList();

            var groupedServiceEvents = GroupEventsBasedOnService(eventsByTime);

            foreach(var keyVal in groupedServiceEvents)
            {
                var service = keyVal.Value.First().Service;
                var events = keyVal.Value;
                
                if (service.ApplyDiscount == 0)
                {
                    var revenue = GetServiceRevenueOf30Days(service.Id);

                    if (!IsCompanyMoreProfitable(events, revenue))
                    {
                        AddDiscountByService(service.Id, service.Price * 0.1);
                    }
                } 
                else
                {
                    RemoveDiscountByService(service.Id);
                }
            }

            return View("./Views/Home/ServiceProviderHomePage.cshtml");
        }

        private double GetServiceRevenueOf30Days(int serviceId)
        {
            var service = _context.Services
                !.Include(s => s.Events)
                .ThenInclude(e => e.Order)
                .Single(s => s.Id == serviceId);

            return service.Events
                .Where(e => e.Order != null && e.Order.Payment != null && e.From.AddDays(30) >= DateTime.Now)
                .Select(e => e!.Order.Sum)
                .Sum();
        }

        private void AddDiscountByService(int serviceId, double discount)
        {
            var service = _context.Services
              !.Include(s => s.Events)
              .ThenInclude(e => e.Order)
              .Single(s => s.Id == serviceId);
            service.ApplyDiscount = discount;
            _context.SaveChanges();
        }

        private void RemoveDiscountByService(int serviceId)
        {
            var service = _context.Services
               !.Include(s => s.Events)
               .ThenInclude(e => e.Order)
               .Single(s => s.Id == serviceId);
            service.ApplyDiscount = 0;
            _context.SaveChanges();
        }
        private bool IsCompanyMoreProfitable(List<Event> events, double revenue)
        {
            double revenueOf5Days = events
                .Where(e => e.Order != null && e.Order.Payment != null && e.From.AddDays(5) >= DateTime.Now && e.From < DateTime.Now)
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
