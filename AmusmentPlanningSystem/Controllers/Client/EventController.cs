using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;

namespace AmusmentPlanningSystem.Controllers.Client
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }
        public IActionResult OpenOrderedEventsList()
        {
            //here we will take client from session
            var client = new Models.Client { UserId = 1, IsBlocked = false, Address = "K. Baršausko g. 67" };
            var orders = _context.Orders.Where(order => order.ClientId == client.UserId).ToList();
            var events = _context.Events.Include(eve => eve.Service).ToList().Join(orders, e => e.OrderId, order => order.Id, (e, order) => e).ToList();

            return View("./Views/OrderedEvents/OrderedEventsList.cshtml", events);
        }

        public List<Event> GetAllEventsByTime(DateTime start, DateTime end)
        {
            return _context
                .Events
                .Include(e => e.Order)
                .Include(e => e.Service)
                .Where(e => e.From <= start && e.To >= end)
                .ToList();
        }
    }
}
