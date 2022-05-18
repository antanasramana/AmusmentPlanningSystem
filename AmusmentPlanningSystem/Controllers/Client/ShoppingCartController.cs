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
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Models.Client _client = new Models.Client { UserId = 1, IsBlocked = false, Address = "K. Baršausko g. 67" };
        public ShoppingCartController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }
        public IActionResult ShowShoppingCart()
        {
            //here we will take client from session

            var shoppingCart = _context.ShoppingCarts.Where(cart => cart.ClientId == _client.UserId).FirstOrDefault();
            var events = _context.Events
                .Where(e => e.ShoppingCartId == shoppingCart.Id)
                .Include(e => e.Service)
                .Include(e => e.Service.Category)
                .ToList();

            return View("./Views/ShoppingCart/ShoppingCartPage.cshtml", events);
        }
        public IActionResult RemoveEventFromCart(int? id)
        {
            _context.Events.Find(id).ShoppingCartId = null;
            _context.SaveChanges();
            ViewData["Success"] = "Events deleted from shoping cart";

            var shoppingCart = _context.ShoppingCarts.Where(cart => cart.ClientId == _client.UserId).FirstOrDefault();
            var events = _context.Events.Where(e => e.ShoppingCartId == shoppingCart.Id).Include(e => e.Service).Include(e => e.Service.Category).ToList();
            return View("./Views/ShoppingCart/ShoppingCartPage.cshtml", events);
        }

        public IActionResult SendSelectedEvent(int? id)
        {
            var @event = _context.Events.Include(e => e.Service).First(e => e.Id == id);

            if (!ValidateSelectedData(@event))
            {
                ViewData["Failure"] = "Failed to order an event - it is already taken";
                return View("./Views/Services/ServiceOrderingPage.cshtml");
            }
            var cart = _context.ShoppingCarts.Where(cart => cart.ClientId == _client.UserId).FirstOrDefault();
            
            @event.ShoppingCartId = cart.Id;
            _context.SaveChanges();

            ViewData["Success"] = "Event added to cart";
            return View("./Views/Services/ServiceOrderingPage.cshtml");
        }

        private bool ValidateSelectedData(Event @event)
        {
            return @event.OrderId == null;
        }

        public IActionResult OpenServiceOrderPage(int? id)
        {
            var events = _context.Events.Include(eve => eve.Service).Where(eve => eve.Service.Id == id && eve.OrderId == null).ToList();

            return View("./Views/Services/ServiceOrderingPage.cshtml", events);
        }

    }

}

