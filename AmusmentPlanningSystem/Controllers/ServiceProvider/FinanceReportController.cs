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
                new SelectListItem("Cash", ((int)MethodOfPayment.Cash).ToString()),
                new SelectListItem("Bank Transfer", ((int)MethodOfPayment.Bank_transfer).ToString()),
            };
            return View("./Views/Reports/MonthlyFinanceReportPage.cshtml", new ReportFilterModel { MethodOfPayment = null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendFilterData([Bind("ServiceName,MethodOfPayment")] ReportFilterModel filters)
        {
            ViewData["Payments"] = new List<SelectListItem>() {
                new SelectListItem("Both", "-1"),
                new SelectListItem("Cash", ((int)MethodOfPayment.Cash).ToString()),
                new SelectListItem("Bank Transfer", ((int)MethodOfPayment.Bank_transfer).ToString()),
            };

            if (!AreFiltersValid(filters))
            {
                ViewData["Error"] = "Incorrect data";
                return View("./Views/Reports/MonthlyFinanceReportPage.cshtml", new ReportFilterModel { MethodOfPayment = null });
            }

            var services = _context.Services!
               .Include(service => service.Events)
                    .ThenInclude(e => e.Order)
                    .ThenInclude(order => order!.Payment)
               .ToList();

            services.ForEach(service => service.Events.ForEach(e => e.Service = service));

            var events = services
                .SelectMany(service => service.Events)
                .Where(e => e.Order != null && e.Order.Payment != null)
                .Where(e => e.From.Month == DateTime.Now.Month)
                .Where(e => 
                    (
                        (
                            filters.MethodOfPayment == "-1" 
                            || 
                            e.Order.MethodOfPayment == (MethodOfPayment)Enum.ToObject(typeof(MethodOfPayment), int.Parse(filters.MethodOfPayment)))
                        ) 
                     && 
                        e.Service.Name.Contains(filters.ServiceName ?? "")
                    )
                .ToList();

            ViewData["Events"] = events;

            return View("./Views/Reports/MonthlyFinanceReportPage.cshtml", new ReportFilterModel { MethodOfPayment = null });
        }

        private bool AreFiltersValid(ReportFilterModel filters)
        {
            if (filters.MethodOfPayment == null)
            {
                return false;
            }

            return true;
        }
    }
}
