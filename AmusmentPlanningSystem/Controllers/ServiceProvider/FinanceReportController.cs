using AmusmentPlanningSystem.Data;
using Microsoft.AspNetCore.Mvc;
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

            return View("./Views/Reports/MonthlyFinanceReportPage.cshtml");
        }

        private bool AreFiltersValid()
        {
            return true;
        }
    }
}
