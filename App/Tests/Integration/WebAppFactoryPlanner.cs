using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Models.Identity;
using App.Server.Util;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Tests.Integration
{
    using LockModel = App.Server.Models.AppData.Lock;
    public struct UserInfo
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public int? RangerId { get; set; }
    };


    public class SeededData
    {
        public List<District> Districts { get; set; } = new List<District>();
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public List<Ranger> Rangers { get; set; } = new List<Ranger>();
        public List<Attendence> Attendences { get; set; } = new List<Attendence>();
        public List<LockModel> Locks { get; set; } = new List<LockModel>();
        public List<Route> Routes { get; set; } = new List<Route>();
        public List<Plan> Plans { get; set; } = new List<Plan>();
    }

    /// <summary>
    /// From article on Integration tests from microsoft : https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
    /// </summary>
    /// <typeparam name="TProgram"></typeparam>
    public class WebAppFactoryPlanner<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {

        public readonly Dictionary<string, UserInfo> UserData = new Dictionary<string, UserInfo>
        {
            ["Admin"] = new UserInfo { Email = "admin@email.com", Password="tempPass.123", RangerId= null},
            ["Ranger"] = new UserInfo { Email = "ranger@email.com", Password = "tempPass.123", RangerId=1 },
            ["HeadOfDistrict"] = new UserInfo { Email = "head@email.com", Password = "tempPass.123", RangerId=2 },
        };
        public DateOnly Date => new DateOnly(2024, 1, 1);

        public SeededData SeededData { get; private set; } = new SeededData();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name &&
                        level >= LogLevel.Warning);  // This will only log warnings or errors
                });

                // remove real databases
                var descriptor1 = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PlannerNPContext>));
                if (descriptor1 != null) services.Remove(descriptor1);

                var descriptor2 = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationIdentityDbContext>));
                if (descriptor2 != null) services.Remove(descriptor2);

                // add in memory databases
                services.AddDbContext<PlannerNPContext>(options =>
                {
                    options.UseInMemoryDatabase("TestPlannerNP")
                    .UseLoggerFactory(loggerFactory);
                });

                services.AddDbContext<ApplicationIdentityDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestIdentity")
                    .UseLoggerFactory(loggerFactory);

                });

                // check creation and seed with data
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<PlannerNPContext>();
                    db.Database.EnsureCreated();

                    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

                    identityDb.Database.EnsureCreated();
                    SeedPlannerDatabase(db);
                    SeedIdentityDatabase(identityDb, sp);
                }
            });
        }

        private async void  SeedPlannerDatabase(PlannerNPContext db)
        {
            // Create 2 Districts
            var district1 = new District { Id = 1, Name = "District1" };
            var district2 = new District { Id = 2, Name = "District1" };

            db.Districts.AddRange(district1, district2);
            await db.SaveChangesAsync();

            // Create Vehicles
            var vehicle1 = new Vehicle
            {
                Id = 1,
                Name = "Vehicle1",
                Type = "Truck",
                DistrictId = 1
            };
            var vehicle2 = new Vehicle
            {
                Id = 2,
                Name = "Vehicle2",
                Type = "Car",
                DistrictId = 1
            };

            db.Vehicles.AddRange(vehicle1, vehicle2);
            await db.SaveChangesAsync();


            // Create Rangers
            var ranger1 = new Ranger
            {
                Id = UserData["Ranger"].RangerId ?? 1,
                FirstName = "firstName1",
                LastName = "lastName1",
                Email = UserData["Ranger"].Email,
                DistrictId = 1
            };
            var ranger2 = new Ranger
            {
                Id = UserData["HeadOfDistrict"].RangerId ?? 2,
                FirstName = "firstname2",
                LastName = "lastname2",
                Email = UserData["HeadOfDistrict"].Email,
                DistrictId = 1
            };

            db.Rangers.AddRange(ranger1, ranger2);
            await db.SaveChangesAsync();


            // Create Attendances
            var attendence1 = new Attendence
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                RangerId = UserData["Ranger"].RangerId ?? 1,
                Working = false,
                ReasonOfAbsenceEnum = ReasonOfAbsence.N
            };
            var attendence2 = new Attendence
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                RangerId = UserData["HeadOfDistrict"].RangerId ?? 2,
                Working = false,
                ReasonOfAbsenceEnum = ReasonOfAbsence.D
            };

            db.Attendences.AddRange(attendence1, attendence2);
            await db.SaveChangesAsync();


            // Create Locks
            var lock1 = new LockModel
            {
                Date = Date,
                DistrictId = 1
            };

            db.Locks.Add(lock1);
            await db.SaveChangesAsync();


            // Create Routes
            var route1 = new Route
            {
                Id = 1,
                Name = "Route 1",
                Priority = 1,
                DistrictId = 1,
                ControlPlace = new ControlPlace { ControlTime = "12:00", ControlPlaceDescription="place" }
            };
            var route2 = new Route
            {
                Id = 2,
                Name = "Route2",
                Priority = 2,
                DistrictId = 1,
                ControlPlace = null
            };

            db.Routes.AddRange(route1, route2);
            await db.SaveChangesAsync();

            // Create Routes
            var plan1 = new Plan
            {
                Date = Date,
                Ranger = ranger1,
                Vehicles = [vehicle1],
                Routes = [route1]

            };
            var plan2 = new Plan
            {
                Date = Date,
                Ranger = ranger2,
                Vehicles = [],
                Routes = [route2]
            };

            db.Plans.AddRange(plan1, plan2);
            await db.SaveChangesAsync();

            SeededData = new SeededData
            {
                Districts = new List<District> { district1, district2 },
                Vehicles = new List<Vehicle> { vehicle1, vehicle2 },
                Rangers = new List<Ranger> { ranger1, ranger2 },
                Attendences = new List<Attendence> { attendence1, attendence2 },
                Locks = new List<LockModel> { lock1 },
                Routes = new List<Route> { route1, route2 },
                Plans = new List<Plan> { plan1, plan2 }
            };
        }



        private void SeedIdentityDatabase(ApplicationIdentityDbContext identityDb, IServiceProvider sp)
        {
            DataSeeder.SeedRolesAsync(sp).Wait();
            foreach (var user in UserData) { 
                SeedUserAsync(sp, user.Value.Email, user.Value.Password, user.Key, user.Value.RangerId).Wait();
           
            }
        }

        public static async Task SeedUserAsync(IServiceProvider serviceProvider, string email, string password, string role, int? rangerId)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(email);

            // already exists
            if (user != null)
            {
                return;
            }

            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                RangerId =  rangerId
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }

}