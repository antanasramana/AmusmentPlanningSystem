using Microsoft.EntityFrameworkCore;
using AmusmentPlanningSystem.Models;

namespace AmusmentPlanningSystem.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                context.Database.EnsureCreated();

                if (!context.Clients.Any())
                {
                    context.Clients.AddRange(
                        new Models.Client()
                        {
                            IsBlocked = false,
                            Address = "Vydūno alėja",
                            Name = "Domas",
                            Surname = "Bainoris",
                            PhoneNumber = "+37070060000",
                            Email = "domas@gmail.com",
                            Password = "saasfdasdasasddasasd"
                        }
                    );
                    context.SaveChanges();
                }

                if (!context.ShoppingCarts.Any())
                {
                    context.ShoppingCarts.AddRange(new ShoppingCart { ClientId = 1 });
                    context.SaveChanges();
                }


                if (!context.ServiceProviders.Any())
                {
                    context.ServiceProviders.AddRange(
                        new Models.ServiceProvider() { 
                            Name = "Agnė", 
                            Surname = "Vlinskytė", 
                            PhoneNumber = "+37070060000", 
                            Email = "agne@gmail.com", 
                            Password = "saasfdasdasasddasasd" 
                        }
                    );
                    context.SaveChanges();
                }

                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { Name="Futbolas" },
                        new Category { Name = "Krepšinis" }
                    );
                    context.SaveChanges();
                }

                if (!context.Companies.Any())
                {
                    context.Companies.AddRange(
                        new Company { Name="Untanis ir Co", CreationDate=DateTime.Now, Adress="K. Petrausko g.",
                                IsClosed=false, ServiceProviderId=2, Logo="SDF2SDF513DSF4"}
                    );
                    context.SaveChanges();
                }

                if (!context.Services.Any())
                {

                    context.Services.Add(new Service
                    {
                        Address = "Vydūno alėja 24",
                        CategoryId = 1,
                        Description = "Realy good service",
                        Price = 450,
                        Name = "Tikriausiai Krešinis pas Untanį",
                        CreationDate = DateTime.Now,
                        EditDate = DateTime.Now,
                        CompanyId = 1
                    });


                    context.Services.Add(new Service { Address="Vydūno alėja 24", CategoryId=1, Description="Better service",
                        Price=500, Name="Futbolas pas null", CreationDate=DateTime.Now, EditDate=DateTime.Now, CompanyId=1});
                    context.SaveChanges();
                }

                if (!context.Workers.Any())
                {
                    context.Workers.Add(new Worker
                    {
                        Name = "Leonis",
                        Surname = "Leoniukas",
                        CompanyId = 1,
                    });
                    context.Workers.Add(new Worker
                    {
                        Name = "Jurga",
                        Surname = "Mantelaitė",
                        CompanyId = 1,
                    });
                    context.SaveChanges();
                }

                if (!context.ServicesWorkers.Any())
                {

                }

                context.SaveChanges();
            }
        }
    }
}

