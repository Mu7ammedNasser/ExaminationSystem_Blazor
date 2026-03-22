using BlazorApp8.Application.Interfaces;
using BlazorApp8.Application.Services;
using BlazorApp8.Components;
using BlazorApp8.Domain.Entities;
using BlazorApp8.Infrastructure.Data;
using BlazorApp8.Infrastructure.Repositories;
using BlazorApp8.Infrastructure.Seed;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp8
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Setup EF Core
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Setup Identity
            builder.Services.AddIdentityCore<ApplicationUser>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();
            
            builder.Services.AddAuthorization();
            builder.Services.AddCascadingAuthenticationState();

            // DI Repositories & Services
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IInstructorService, InstructorService>();
            builder.Services.AddScoped<IStudentService, StudentService>();

            var app = builder.Build();

            // Seed Data
            using (var scope = app.Services.CreateScope())
            {
                var scopedContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    scopedContext.Database.Migrate();
                    
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    
                    await IdentitySeeder.SeedAsync(userManager, roleManager);
                    DataSeeder.SeedExamsAndQuestions(scopedContext);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
