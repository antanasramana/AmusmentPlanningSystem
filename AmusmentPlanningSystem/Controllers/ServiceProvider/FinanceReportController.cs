using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmusmentPlanningSystem.Controllers.ServiceProvider
{
    public class FinanceReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinanceReportController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }

        public IActionResult ShowMonthlyReport()
        {
            ViewData["Payments"] = new List<SelectListItem>() {
                new SelectListItem("Both", "-1"),
                new SelectListItem("Cash", MethodOfPayment.Cash.ToString()),
                new SelectListItem("Bank Transfer", MethodOfPayment.Bank_transfer.ToString()),
            };
            return View("./Views/Reports/MonthlyFinanceReportPage.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendFilterData()
        {
            if (!AreFiltersValid())
            {
                ViewData["Error"] = "Incorrect data";
                return View("./Views/ProvidersServices/AddServicePage.cshtml");
            }

            var services = _context.Service!
               .Include(service => service.Events)
                    .ThenInclude(e => e.Order)
                    .ThenInclude(order => order!.Payment)
               .ToList();

            services.ForEach(service => service.Events.ForEach(e => e.Service = service));

            var events = services
                .SelectMany(service => service.Events)
                .ToList();

            ViewData["Events"] = events;
            ViewData["Payments"]= new List<SelectListItem>() {
                new SelectListItem("Both", "-1"),
                new SelectListItem("Cash", MethodOfPayment.Cash.ToString()),
                new SelectListItem("Cash", MethodOfPayment.Bank_transfer.ToString()),
            };

            return View("./Views/Reports/MonthlyFinanceReportPage.cshtml");
        }

        private bool AreFiltersValid()
        {
            return true;
        }
    }
}
