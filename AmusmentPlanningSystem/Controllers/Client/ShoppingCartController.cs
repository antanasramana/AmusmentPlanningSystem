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

        public ShoppingCartController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }
        public IActionResult ShowShoppingCart()
        {
            //here we will take client from session
            var client = new Models.Client { UserId = 1, IsBlocked = false, Address = "K. Baršausko g. 67" };
            var shoppingCart = _context.ShoppingCarts.Where(cart => cart.ClientId == client.UserId).FirstOrDefault();
            var events = _context.Events
                .Where(e => e.ShoppingCartId == shoppingCart.Id)
                .Include(e => e.Service)
                .Include(e=>e.Service.Category)
                .ToList();

            return View("./Views/ShoppingCart/ShoppingCartPage.cshtml", events);
        }
        public IActionResult RemoveEventFromCart(int? id)
        {
            _context.Events.Find(id).ShoppingCartId = null;
            _context.SaveChanges();
            ViewData["Success"] = "Events deleted from shoping cart";
            var client = new Models.Client { UserId = 1, IsBlocked = false, Address = "K. Baršausko g. 67" };
            var shoppingCart = _context.ShoppingCarts.Where(cart => cart.ClientId == client.UserId).FirstOrDefault();
            var events = _context.Events.Where(e => e.ShoppingCartId == shoppingCart.Id).Include(e => e.Service).Include(e => e.Service.Category).ToList();
            return View("./Views/ShoppingCart/ShoppingCartPage.cshtml", events);
        }


    }
}
