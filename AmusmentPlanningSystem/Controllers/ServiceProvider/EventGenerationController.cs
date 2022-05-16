using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AmusmentPlanningSystem.Controllers
{
    public class EventGenerationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly int companyId = 1;
        public EventGenerationController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }



        [HttpPost]

        public IActionResult SendData(int serviceId, DateTime start, DateTime end, int duration, string workHourStart, string workHourEnd)
        {
            var workers = _context.ServicesWorkers.Where(w => w.ServiceId == serviceId)
                                .Join(_context.Workers,
                                sw => sw.WorkerId,
                                w => w.Id,
                                (sw, w) => w).ToList();

            var events = new List<Event>();

            if (WorkersExist(workers))
            {
                foreach (var worker in workers)
                {
                    var newEvents = CreateEvents(serviceId, start, end, duration);
                    AssignWorker(newEvents, worker.Id);

                    events.AddRange(newEvents);
                }
            }
            else
            {
                var newEvents = CreateEvents(serviceId, start, end, duration);
                events.AddRange(newEvents);
            }

            int i = 0;
            while (i<events.Count)
            {
                var e = events[i];

                List<Task<bool>> tasks = new List<Task<bool>>();

                if (e.WorkerId==null)
                {
                    tasks.Add(Task.Factory.StartNew(() => CheckTimeClash()));
                }
                tasks.Add(Task.Factory.StartNew(() => CheckHoliday()));
                tasks.Add(Task.Factory.StartNew(() => CheckHours()));

                Task.WaitAll(tasks.ToArray());

                var results = SaveValidationData(tasks);

                if (AtleastOneIsTrue(results))
                {
                    RemoveEventFromList(i, events);
                    i--;
                }

                i++;
            }

            _context.Events.AddRange(events);

            _context.SaveChanges();


            ViewData["events"] = events;
            ViewData["serviceId"] = serviceId;
            ViewData["generated"] = true;
            return View("./Views/ProvidersServices/EventGenerationPage.cshtml");

        }
        public IActionResult ConfirmEvents()
        {
            var services = _context.Services.Include(c => c.Category).Where(s=>s.CompanyId==companyId).ToList();
            return View("./Views/ProvidersServices/ProvidersServiceList.cshtml", services);
        }

        [HttpGet]
        public IActionResult DeleteEvent(int id)
        {
            var eventForDeletion = _context.Events.Find(id);
            int serviceId=eventForDeletion.ServiceId;
            _context.Events.Remove(eventForDeletion);
            _context.SaveChanges();


            ViewData["events"] = _context.Events.Where(e => e.ServiceId == serviceId).ToList();
            ViewData["serviceId"] = serviceId;
            ViewData["generated"] = true;

            return View("./Views/ProvidersServices/EventGenerationPage.cshtml");

        }

        private void RemoveEventFromList( int index, List<Event> events)
        {
            events.RemoveAt(index);
        }

        private List<bool> SaveValidationData(List<Task<bool>> tasks)
        {
            return tasks.Select(task => task.Result).ToList();
        }

        private bool AtleastOneIsTrue(List<bool> results)
        {
            return results.Contains(true);
        }

        private bool CheckTimeClash()
        {
            return false;
        }
        private bool CheckHoliday()
        {
            return false;
        }
        private bool CheckHours()
        {
            return false;
        }


        public IActionResult OpenEventGenerationPage(int id)
        {
            ViewData["serviceId"] =id;

            ViewData["events"] = new List<Event>();
            ViewData["generated"] = false;
            return View("./Views/ProvidersServices/EventGenerationPage.cshtml");
        }

        private List<Event> CreateEvents(int id, DateTime Start, DateTime End, int duration)
        {
            List<Event> events = new List<Event>();

            var currDate = Start;
            while (currDate < End)
            {
                var newDate = currDate.AddMinutes(duration);

                var newEvent = new Event{  Discount=0, From=currDate, To=newDate, ServiceId=id};
                events.Add(newEvent);

                currDate= newDate;
            }
            return events;
        }
        private void AssignWorker(List<Event> events, int workerId)
        {
            foreach (var e in events)
            {
                e.WorkerId = workerId;
            }
        }
        private bool WorkersExist(List<Worker> workers)
        {
            return workers.Count != 0;
        }
    }
}
