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
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Company>? Companies { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Service>? Services { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Models.ServiceProvider> ServiceProviders { get; set; }
        public DbSet<ServiceWorker> ServicesWorkers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set;}
        public DbSet<Worker> Workers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<ServiceWorker>().HasOne(s => s.Service)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ServiceWorker>().HasOne(s => s.Worker)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<User>()
                .ToTable("Users");
            builder.Entity<Worker>()
                .ToTable("Workers");
            builder.Entity<Client>()
                .ToTable("Clients");
            builder.Entity<Models.ServiceProvider>()
                .ToTable("ServiceProviders");
            builder.Entity<Models.Company>()
                .ToTable("Companies");
        }
    }
}