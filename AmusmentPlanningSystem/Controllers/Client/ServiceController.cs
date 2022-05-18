using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Mocks;
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
        private readonly IGoogleService _googleService;

        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
            _googleService=new GoogleService();
        }

        [HttpGet]
        [Route("/service")]
        public IActionResult ShowRecommendedServices()
        {
            var servicesToDisplay = RecommendServices();

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

            ViewData["ClientRating"] = GetClientOpenedServiceRating(client, id);

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

            var service = _context.Services!
                .Single(service => service.Id == id);
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

                client.Ratings.Add(newRating);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(OpenServiceInformation), new { id });
        }

        private int? GetClientOpenedServiceRating(Models.Client client, int serviceId)
        {
            return client.Ratings.SingleOrDefault(rating => rating.Service.Id == serviceId)?.Evaluation;
        }

        private IEnumerable<Service> RecommendServices()
        {
            // TODO: Get logged in user
            var client = _context.Clients!
                .Include(client => client.Orders)
                    .ThenInclude(order => order.Events)
                    .ThenInclude(events => events.Service)
                    .ThenInclude(service => service.Category)
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
                    .DistinctBy(c => c.Id)
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

            var addressDistances = _googleService.GetDistanceMatrix(services, client);

            return SortServicesByDistance(services, addressDistances);
        }

        private List<Service> SortServicesByRating(IEnumerable<Service> services)
        {
            return services.OrderByDescending(service =>
            {
                if (service.Ratings.Any())
                {
                    return service.Ratings.Average(rating => rating.Evaluation);
                }
                else
                {
                    return 0;
                }
            }).ToList();
        }

        private IEnumerable<Service> SortServicesByDistance(IEnumerable<Service> services, IEnumerable<int> distances)
        {
            return services.Zip(distances).OrderByDescending(pair => pair.Second).Select(pair => pair.First);
        }
    }
}
