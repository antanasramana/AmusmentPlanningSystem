using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;

namespace AmusmentPlanningSystem.Controllers.ServiceProvider
{
    public class ProvidersServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Company company = new Company() { Id=1};
        public ProvidersServicesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }

        public IActionResult ShowProvidersServices()
        {
            var services = _context.Services.Include(c => c.Category).ToList();
            return View("./Views/ProvidersServices/ProvidersServiceList.cshtml", services);
        }

        public IActionResult ShowServiceCreation()
        {
            var items = _context.Categories.ToList();
            var workers = _context.Workers.Where(c => c.CompanyId== company.Id).ToList();
            var categories = items.Select(item => new SelectListItem(item.Name, item.Id.ToString())).ToList();
            ViewData["Categories"] = categories;
            ViewData["id"] = -1;
            ViewData["Workers"] = workers;
            return View("./Views/ProvidersServices/AddServicePage.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendServiceData([Bind("Id,Name,Address,Description,Picture,Price,EditDate,CategoryId,CreationDate")] Service service, int [] workers)
        {
            var items = _context.Categories.ToList();
            var categories = items.Select(item => new SelectListItem(item.Name, item.Id.ToString())).ToList();
            ViewData["Categories"] = categories;
            ViewData["id"] = -1;


            if (!ValidateServiceData(service))
            {
                ViewData["Error"] = "Incorrect data";
                ViewData["id"] = -1;
                return View("./Views/ProvidersServices/AddServicePage.cshtml");
            }

            service.CompanyId= company.Id;
            service.CreationDate = DateTime.Now;
            service.EditDate = DateTime.Now;
            var addedService = _context.Add(service).Entity;
            _context.SaveChanges();

            foreach (var id in workers)
            {
                _context.ServicesWorkers.Add(new ServiceWorker { ServiceId= addedService.Id, WorkerId = id});
            }
            _context.SaveChanges();



            ViewData["Success"] = "Correct data";
            ViewData["Workers"] = _context.ServicesWorkers.Where(w=>w.ServiceId==addedService.Id)
                                            .Join(_context.Workers,
                                            sw=>sw.WorkerId,
                                            w=>w.Id, 
                                            (sw,w)=> w).ToList();
            ViewData["id"] = addedService.Id;

            return View("./Views/ProvidersServices/AddServicePage.cshtml", service);

        }

        public IActionResult EditService(int? id)
        {
            var service = _context.Services.Find(id);

            var items = _context.Categories.ToList();
            var categories = items.Select(item => new SelectListItem(item.Name, item.Id.ToString())).ToList();
            ViewData["Categories"] = categories;

            return View("./Views/ProvidersServices/EditServicePage.cshtml", service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditServiceData(int id, [Bind("Id,Name,Address,Description,Picture,Price,EditDate,CategoryId, CreationDate")] Service service)
        {


            if (!ValidateServiceData(service))
            {
                ViewData["Error"] = "Incorrect data";
                var items = _context.Categories.ToList();
                var categories = items.Select(item => new SelectListItem(item.Name, item.Id.ToString())).ToList();
                ViewData["Categories"] = categories;
                return View("./Views/ProvidersServices/EditServicePage.cshtml", service);
            }

            service.EditDate = DateTime.Now;
            _context.Update(service);
            _context.SaveChanges();

            var services = _context.Services.Include(c => c.Category).ToList();
            return View("./Views/ProvidersServices/ProvidersServiceList.cshtml", services);
        }

        public IActionResult DeleteService(int? id)
        {
            var service = _context.Services.Find(id);
            return View("./Views/ProvidersServices/DeleteServicePage.cshtml", service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDeletion(int id)
        {
            var events = _context.Events.ToList();
            var service = _context.Services.Find(id);

            //Service Workers needed

            if (!ValidateDeletion(events))
            {
                ViewData["Error"] = "Service cannot be deleted, because it has events";
                return View("./Views/ProvidersServices/DeleteServicePage.cshtml", service);
            }

            _context.Services.Remove(service);
            _context.SaveChanges();


            var services = _context.Services.Include(c => c.Category).ToList();
            return View("./Views/ProvidersServices/ProvidersServiceList.cshtml", services);
        }



        public bool ValidateServiceData(Service service)
        {
            return service.Price > 0;
        }
        public bool ValidateDeletion(List<Event> events)
        {
            return events.Count == 0;
        }
    }
}
