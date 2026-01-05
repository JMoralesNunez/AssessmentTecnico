using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager)
    {
        // Seed Test User
        var testUserEmail = "test@user.com";
        var user = await userManager.FindByEmailAsync(testUserEmail);

        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = testUserEmail,
                Email = testUserEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "Password123!");
        }
    }
}
