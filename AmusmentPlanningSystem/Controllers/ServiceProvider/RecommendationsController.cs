using AmusmentPlanningSystem.Data;
using AmusmentPlanningSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AmusmentPlanningSystem.Controllers.ServiceProvider
{
    public class RecommendationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Models.ServiceProvider _serviceProvider = new Models.ServiceProvider { UserId = 3 };

        public RecommendationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recommendations
        public async Task<IActionResult> ShowCompanies()
        {
            var companies = _context.Companies.Include(c => c.ServicesProvider).Where(x => x.ServiceProviderId == _serviceProvider.UserId).ToList();

            return View("./Views/Recommendations/CompanyList.cshtml", companies);
        }

        // GET: Recommendations/Details/5
        public async Task<IActionResult> ShowCompanyRecommendations(int? id)
        {
            var company = _context.Companies
                .Include(c => c.ServicesProvider)
                .Where(m => m.Id == id).First();

            var companyCategories = _context.Services.Where(x => x.CompanyId == company.Id).Include(x => x.Category).Select(x => x.Category);
            
            var priceToUseList = new List<double>();
            var priceDeviationFromMeanList = new List<double>();
            var currentCategories = new List<Category>();
            var categoryToUseList = new List<Category>();

            foreach(var category in companyCategories)
            {
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var services = _context.Services.Include(x => x.Category).Where(x => x.Category.Id == category.Id).ToList();
                    var priceStandartDevation = CalculatePriceStandardDeviation(services);
                    var priceToUse = CalculatePriceMean(services);

                    priceToUseList.Add(priceToUse);
                    priceDeviationFromMeanList.Add(priceStandartDevation);
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var mostPopularCategoriesFromCompanies = new List<Category>();
                    var companiesThatServeSameCategory = _context.Services.Include(x => x.Company)
                    .Where(x => x.Category.Id == category.Id && x.CompanyId != company.Id)
                    .Select(x => x.Company).ToList();

                    foreach (var company in companiesThatServeSameCategory)
                    {
                        var companysServices = _context.Services.Include(x => x.Category).Where(x => x.CompanyId == company.Id).ToList();
                        var categoriesOfServices = companysServices.Select(x => x.Category).ToList();
                        var mostPopularCategory = FindMostPopularCategory(categoriesOfServices, category);

                        mostPopularCategoriesFromCompanies.Add(mostPopularCategory);
                    }

                    var mostPopularCategoryFromAll = FindMostPopularCategory(mostPopularCategoriesFromCompanies, category);
                    categoryToUseList.Add(mostPopularCategoryFromAll);
                }));

                Task.WaitAll(tasks.ToArray());

                currentCategories.Add(category);
            }

            ViewData["categoriesToUse"] = categoryToUseList;
            ViewData["currentCategories"] = currentCategories;
            ViewData["priceToUse"] = priceToUseList;
            ViewData["priceDeviation"] = priceDeviationFromMeanList;

            return View("./Views/Recommendations/RecommendationsPage.cshtml");
        }

        private double CalculatePriceMean(List<Service> services)
        {
            return services.Average(x => x.Price);
        }

        private double CalculatePriceStandardDeviation(List<Service> services)
        {
            var priceAverage = services.Average(x => x.Price);
            return Math.Sqrt(services.Average(s => Math.Pow(s.Price - priceAverage , 2)));
        }

        private Category FindMostPopularCategory(List<Category> categories, Category categoryToExclude)
        {
            var categoriesWithoutExcluded = categories.Where(x => x.Id != categoryToExclude.Id).ToList();

            int maxCategoryCount = _context.Services.Where(x => x.Id != categoryToExclude.Id).Count(x => x.CategoryId == categoriesWithoutExcluded[0].Id);
            Category popularCategory = categoriesWithoutExcluded[0];
            foreach(var category in categoriesWithoutExcluded)
            {
                var countOfServicesByCategory = _context.Services.Count(x => x.CategoryId == category.Id);
                if (countOfServicesByCategory > maxCategoryCount)
                {
                    maxCategoryCount = countOfServicesByCategory;
                    popularCategory = category;
                }
            }
            return popularCategory;
        }
    }
}
