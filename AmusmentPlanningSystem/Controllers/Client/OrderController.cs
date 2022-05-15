using System;
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
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BankInterface _bankInterface;
        public OrderController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _bankInterface = new Bank();
        }
        public IActionResult OrderEvents(int[] events)
        {
            //here we will take client from session
            var client = new Models.Client { UserId = 1, IsBlocked = false, Address = "K. Baršausko g. 67" };
            var order = _context.Orders.Add(new Order { Sum = 0, date = DateTime.Now, ClientId = client.UserId, MethodOfPayment = MethodOfPayment.Cash }).Entity;
            _context.SaveChanges();
            var sum = 0.0;
            foreach (var e in events)
            {
                var even = _context.Events.Where(eve=> eve.Id == e).Include(eve => eve.Service).FirstOrDefault();
                _context.Events.Find(e).OrderId = order.Id;
                _context.Events.Find(e).ShoppingCartId = null;
                sum += even.Service.Price;
            }
            
            _context.Orders.Find(order.Id).Sum = sum;
            _context.SaveChanges();

            ViewData["orderId"] = order.Id;
            ViewData["sum"] = (int)sum;
            ViewData["payed"] = false;
            return View("./Views/ShoppingCart/PaymentPage.cshtml");
        }

        [HttpPost]
        public IActionResult SendPaymentData(string password, int orderId, int sum)
        {

            var status = _bankInterface.SendPaymentData(password);

            if (CheckPaymentStatus(status))
            {
                ViewData["generated"] = true;
                ViewData["orderId"] = orderId;
                ViewData["sum"] = sum;
                ViewData["payed"] = true;

                var payment = new Payment { Date = DateTime.Now };
                _context.Add(payment);
                _context.SaveChanges();

                var order = _context.Orders.Find(orderId);
                order.PaymentId = payment.Id;
                order.MethodOfPayment = MethodOfPayment.Bank_transfer;
                _context.SaveChanges();


                return View("./Views/ShoppingCart/PaymentPage.cshtml");
            }
            else
            {
                ViewData["orderId"] = orderId;
                ViewData["sum"] = sum;
                ViewData["payed"] = false;
                ViewData["error"] = "Incorrect data";
                return View("./Views/ShoppingCart/PaymentPage.cshtml");
            }
        }
        
        [HttpPost]
        public IActionResult ReturnToCart()
        {
            return View("./Views/ShoppingCart/ShoppingCartPage.cshtml", new List<Event>());
        }
        private bool CheckPaymentStatus(bool status)
        {
            return status;
        }
    }
}
