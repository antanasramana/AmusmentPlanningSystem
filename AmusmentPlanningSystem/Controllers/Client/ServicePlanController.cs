using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;
using AmusmentPlanningSystem.Mocks;

namespace AmusmentPlanningSystem.Controllers.Client
{
    public class ServicePlanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGoogleService _googleService;

        public ServicePlanController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _googleService = new GoogleService();
        }
        public IActionResult OpenServicePlan()
        {
            var items = _context.Categories.ToList();
            var categories = items.Select(item => new SelectListItem(item.Name, item.Id.ToString())).ToList();
            ViewData["Categories"] = categories;
            ViewData["CategoryCount"] = 1;
            return View("./Views/ServicePlan/ServicePlanPage.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendServicePlanData(DateTime startDateTime, DateTime endDateTime, int[] categories)
        {
            var items = _context.Categories.ToList();
            var allCategories = items.Select(item => new SelectListItem(item.Name, item.Id.ToString())).ToList();
            ViewData["Categories"] = allCategories;

            if (!ValidateData(startDateTime, endDateTime, categories))
            {
                ViewData["Error"] = "All fields required. Start date can't be equal or higher than end date";
                return View("./Views/ServicePlan/ServicePlanPage.cshtml");
            }
            else
            {


                //here we will take client from session
                var client = new Models.Client { UserId = 1, IsBlocked = false, Address = "K. Baršausko g. 67" };

                string addressStart = client.Address;
                DateTime eventFinishDateTime = startDateTime;
                var eventList = new List<Event>();

                foreach (int id in categories)
                {

                    var services = _context.Services.Where(item => item.CategoryId == id).ToList();
                    var events = _context.Events.Where(e => e.OrderId == null)
                        .ToList().Join(services, e => e.ServiceId, service => service.Id, (e, service) => e)
                        .Where(e => e.From >= eventFinishDateTime && e.To <= endDateTime).ToList();

                    TimeSpan shortestTime = new TimeSpan(365, 0, 0);
                    Event eventToAdd = null;

                    foreach (var e in events)
                    {
                        TimeSpan travelTime = _googleService.CalculateTravelTime(addressStart, e.Service.Address);
                        TimeSpan waitingTime = CalculateWaitingTime(startDateTime, e.From);
                        TimeSpan longerTime = EvaluateTravelAndWait(travelTime, waitingTime);
                        if (longerTime < shortestTime)
                        {
                            shortestTime = UpdateShortestTime(longerTime);
                            eventToAdd = e;
                        }
                    }

                    eventList.Add(eventToAdd);

                    if (eventToAdd == null)
                        break;
                    addressStart = eventToAdd.Service.Address;
                    eventFinishDateTime = eventToAdd.To;
                }
                if (!ValidateServicePlan(endDateTime, eventFinishDateTime, eventList))
                {
                    ViewData["Error"] = "Sorry couldn't make a plan for this time :(.";
                    return View("./Views/ServicePlan/ServicePlanPage.cshtml");
                }
                else
                {
                    ViewData["ServicePlan"] = eventList;
                    ViewData["Success"] = "Correct data";
                    return View("./Views/ServicePlan/ServicePlanPage.cshtml");

                }
            }

        }
        public bool ValidateData(DateTime startDateTime, DateTime endDateTime, int[] categories)
        {
            if (startDateTime == null || endDateTime == null || categories.Count() == 0)
                return false;
            return endDateTime >= startDateTime;
        }
        public TimeSpan CalculateWaitingTime(DateTime eventFinishDateTime, DateTime nextEventDateTime)
        {
            return nextEventDateTime - eventFinishDateTime;
        }
        public TimeSpan UpdateShortestTime(TimeSpan newShortestTime)
        {
            return newShortestTime;
        }
        public TimeSpan EvaluateTravelAndWait(TimeSpan travelTime, TimeSpan waitingTime)
        {
            if (travelTime > waitingTime)
            {
                return travelTime;
            }
            else
            {
                return waitingTime;
            }
        }
        public bool ValidateServicePlan(DateTime endDateTime, DateTime eventFinishDateTime, List<Event> eventList)
        {
            return endDateTime >= eventFinishDateTime && !(eventList.Contains(null));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmServicePlan(int[] events)
        {
            var items = _context.Categories.ToList();
            var allCategories = items.Select(item => new SelectListItem(item.Name, item.Id.ToString())).ToList();
            ViewData["Categories"] = allCategories;

            //here we will take client from session
            var client = new Models.Client { UserId = 1, IsBlocked = false, Address = "K. Baršausko g. 67" };
            var shoppingCart = _context.ShoppingCarts.Where(item => item.ClientId == client.UserId).FirstOrDefault();

            foreach (int id in events)
            {
                _context.Events.Find(id).ShoppingCartId = shoppingCart.Id;
                _context.SaveChanges();
            }
            ViewData["Success"] = "Events succesfully added to shopping cart";
            return View("./Views/ServicePlan/ServicePlanPage.cshtml");
        }
    }
}
