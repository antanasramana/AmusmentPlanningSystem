using AmusmentPlanningSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AmusmentPlanningSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/ServiceProvider")]
        public IActionResult ServiceProviderHomePage()
        {
            return View("./Views/Home/ServiceProviderHomePage.cshtml");
        }
        [HttpGet]
        [Route("/Client")]
        public IActionResult ClientHomePage()
        {
            return View("./Views/Home/ClientHomePage.cshtml");
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(ClientHomePage));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}