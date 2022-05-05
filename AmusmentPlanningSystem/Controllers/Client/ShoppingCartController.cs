using AmusmentPlanningSystem.Data;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View();
        }

    }
}
