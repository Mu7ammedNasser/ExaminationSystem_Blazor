using BlazorApp8.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlazorApp8.Infrastructure.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Create roles if they do not exist
            var roles = new[] { "Instructor", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2 & 3. Create instructor account and assign role
            var instructorEmail = "instructor@gmail.com";
            if (await userManager.FindByEmailAsync(instructorEmail) == null)
            {
                var instructorUser = new ApplicationUser
                {
                    UserName = instructorEmail,
                    Email = instructorEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(instructorUser, "instructor123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(instructorUser, "Instructor");
                }
            }

            var studentEmail = "student@gmail.com";
            if (await userManager.FindByEmailAsync(studentEmail) == null)
            {
                var studentUser = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(studentUser, "student123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(studentUser, "Student");
                }
            }
        }
    }
}
