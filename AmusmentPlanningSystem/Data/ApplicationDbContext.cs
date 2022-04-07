using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AmusmentPlanningSystem.Models;

namespace AmusmentPlanningSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Service>? Service { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}