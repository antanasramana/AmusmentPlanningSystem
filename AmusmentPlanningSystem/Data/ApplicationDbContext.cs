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
            base.Database.EnsureCreated();
        }
        public DbSet<Service>? Service { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .ToTable("Users");
            builder.Entity<Client>()
                .ToTable("Clients");
            builder.Entity<Models.ServiceProvider>()
                .ToTable("ServiceProviders");
        }
    }
}