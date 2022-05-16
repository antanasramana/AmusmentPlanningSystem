using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AmusmentPlanningSystem.Controllers.Client
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // TODO: Rename To "ShowServices" in MagicDraw and update this to reflect that
        [HttpGet]
        [Route("/service")]
        public IActionResult ShowClientHomePage()
        {
            var servicesToDisplay = RecommendServices();

            ViewData["Ratings"] = servicesToDisplay.Select(service => service.Ratings.Average(rating => rating.Evaluation).ToString()).ToArray();

            return View("/Views/Services/ServiceList.cshtml", servicesToDisplay);
        }

        [HttpGet]
        [Route("/service/{id}")]
        public IActionResult OpenServiceInformation(int id)
        {
            // TODO: Replace with actual client getter
            var client = _context.Clients
                .Include(client => client.Ratings)
                    .ThenInclude(rating => rating.Service)
                .Single(client => client.UserId == 1);

            var service = _context.Services!
                .Include(service => service.Ratings)
                .Include(service => service.Company)
                .Include(service => service.Comments)
                    .ThenInclude(comment => comment.Client)
                .Single(service => service.Id == id);

            ViewData["ClientRating"] = client.Ratings.SingleOrDefault(rating => rating.Service.Id == id)?.Evaluation;

            return View("/Views/Services/ServicePage.cshtml", service);
        }

        [HttpPost]
        [Route("/service/{id}/rating/{evaluation}")]
        public IActionResult LeaveRating(int id, int evaluation)
        {
            var client = _context.Clients
                .Include(client => client.Ratings)
                    .ThenInclude(rating => rating.Service)
                .Single(client => client.UserId == 1); // TODO: Replace with actual client getter

            var service = _context.Services!.Single(service => service.Id == id);
            var leftRating = client.Ratings.SingleOrDefault(rating => rating.Service.Id == id);

            if (leftRating != null)
            {
                leftRating.Evaluation = evaluation;
            }
            else
            {
                var newRating = new Rating
                {
                    Client = client,
                    Service = service,
                    Evaluation = evaluation,
                };

                service.Ratings.Add(newRating);
                client.Ratings.Add(newRating);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(OpenServiceInformation), new { id });
        }

        private IEnumerable<Service> RecommendServices()
        {
            // TODO: Get logged in user
            var client = _context.Clients!
                .Include(client => client.Orders)
                    .ThenInclude(order => order.Events)
                .Include(client => client.Orders)
                    .ThenInclude(order => order.Events)
                .Single(client => client.UserId == 1);
            var clientOrders = client.Orders;

            List<Service> services = new();

            if (clientOrders.Count == 0)
            {
                services = _context.Services!
                    .Include(service => service.Ratings)
                    .ToList();

                services = SortServicesByRating(services);
            }
            else
            {
                var categories = clientOrders
                    .SelectMany(order => order.Events)
                    .Select(e => e.Service.Category)
                    .ToList();

                for (int i = 0; i < categories.Count; i++)
                {
                    var categoryServices = _context.Categories!
                        .Include(category => category.Services)
                            .ThenInclude(service => service.Ratings)
                        .Single(category => category.Id == categories[i].Id)
                        .Services;

                    var servicesSortedByRating = SortServicesByRating(categoryServices);

                    services.AddRange(servicesSortedByRating);
                }
            }

            var addressDistances = GetDistanceMatrix(services, client);

            return SortServicesByDistance(services, addressDistances);
        }

        private List<Service> SortServicesByRating(IEnumerable<Service> services)
        {
            return services.OrderByDescending(service => service.Ratings.Average(rating => rating.Evaluation)).ToList();
        }

        private IEnumerable<int> GetDistanceMatrix(IEnumerable<Service> services, Models.Client client)
        {
            // request would be https://maps.googleapis.com/maps/api/distancematrix/json?
            //                      destinations={services.Select(service => service.Address).Join('|')}&
            //                      origins={client.Address}&
            //                      key={apiKey}
            // response looks like this: 
            /*    {
                  "destination_addresses": ["San Francisco, CA, USA", "Victoria, BC, Canada"],
                  "origin_addresses": ["Vancouver, BC, Canada", "Seattle, WA, USA"],
                  "rows":
                    [
                      {
                        "elements":
                          [
                            {
                              "distance": { "text": "1,723 km", "value": 1723247 },
                              "duration": { "text": "3 days 21 hours", "value": 335356 },
                              "status": "OK",
                            },
                            {
                              "distance": { "text": "139 km", "value": 138696 },
                              "duration": { "text": "6 hours 47 mins", "value": 24405 },
                              "status": "OK",
                            },
                          ],
                      },
                      {
                        "elements":
                          [
                            {
                              "distance": { "text": "1,468 km", "value": 1468210 },
                              "duration": { "text": "3 days 7 hours", "value": 284548 },
                              "status": "OK",
                            },
                            {
                              "distance": { "text": "146 km", "value": 146500 },
                              "duration": { "text": "2 hours 53 mins", "value": 10376 },
                              "status": "OK",
                            },
                          ],
                      },
                    ],
                  "status": "OK",
                }
            */

            // These are mocks
            var rng = new Random();
            return Enumerable.Range(1, services.Count()).OrderBy(_ => rng.Next());
        }

        private IEnumerable<Service> SortServicesByDistance(IEnumerable<Service> services, IEnumerable<int> distances)
        {
            return services.Zip(distances).OrderByDescending(pair => pair.Second).Select(pair => pair.First);
        }


        public double GetServiceRevenueOf30Days(int serviceId)
        {
            var service = _context.Service
                !.Include(s => s.Events)
                .ThenInclude(e => e.Order)
                .Single(s => s.Id == serviceId);

            return service.Events
                .Where(e => e.From.AddDays(30) >= DateTime.Now)
                .Select(e => e!.Order.Sum)
                .Sum();
        }

        public void AddDiscountByService(int serviceId, double discount)
        {
            var service = _context.Service
                !.Include(s => s.Events)
                .ThenInclude(e => e.Order)
                .Single(s => s.Id == serviceId);
            service.ApplyDiscount = discount;
            _context.SaveChanges();
        }

        public void RemoveDiscountByService(int serviceId)
        {
            var service = _context.Service
                !.Include(s => s.Events)
                .ThenInclude(e => e.Order)
                .Single(s => s.Id == serviceId);
             service.ApplyDiscount = 0;
            _context.SaveChanges();
        }
    }

}
