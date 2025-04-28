using App.Server.CSP;
using App.Server.Models.AppData;
using App.Server.Models.Identity;
using App.Server.Repositories;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using App.Server.Services.Hubs;
using App.Server.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Database context for app data
            builder.Services.AddDbContext<PlannerNPContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("PlannerNPConnection") ?? throw new InvalidOperationException("Connection string not found.");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // Database context for Identity
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection")
                    ?? throw new InvalidOperationException("Connection string not found.");
                options.UseMySql(identityConnectionString, ServerVersion.AutoDetect(identityConnectionString));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddDefaultTokenProviders();


            // For Injecting into PlanController
            builder.Services.AddScoped<IRoutePlanGenerator, RoutePlanGenerator>();

            // Authorization and Authentications
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IAppAuthenticationService, InternalAuthenticationService>();
            builder.Services.AddScoped<IAppAuthorizationService, InternalAuthorizationService>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // SignalR for realTime updates
            builder.Services.AddSignalR();

            // Controllers
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors(x => x
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true) // allow any origin
                    .AllowCredentials());
            }

            // Seeding data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                Task.Run(async () =>
                {
                    await DataSeeder.SeedRolesAsync(services);
                    await DataSeeder.SeedAdminUserAsync(services);
                }).GetAwaiter().GetResult();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<DistrictHub>("/districtHub");
            app.MapHub<RangerScheduleHub>("/rangerScheduleHub");

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
