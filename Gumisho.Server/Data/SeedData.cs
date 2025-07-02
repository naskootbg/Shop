using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var _userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var context = serviceProvider.GetRequiredService<AppDbContext>();

            const string USER_ID = "22e40406-8a9d-2d82-912c-5d6a640ee696";
            const string GUEST_ID = "23e40406-8a9d-2d82-912c-5d6a640ee697";
            if (_roleManager != null)
            {
                IdentityRole? role = await _roleManager.FindByNameAsync("Admin");

                if (role == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Driver"));
                }
            }

            //context.Database.ExecuteSqlRaw("DELETE FROM Images");
            //context.Database.ExecuteSqlRaw("DELETE FROM Orders");
            //context.Database.ExecuteSqlRaw("DELETE FROM UserAddresses");
            //context.Database.ExecuteSqlRaw("DELETE FROM WorkingStates");
            //context.Database.ExecuteSqlRaw("DELETE FROM Services");

            IdentityUser user = new()
            {
                Id = USER_ID,
                UserName = "admin@evtinoo.com",
                NormalizedUserName = "ADMIN@EVTINOO.COM",
                Email = "admin@evtinoo.com",
                NormalizedEmail = "ADMIN@EVTINOO.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEFmQjdr/b3eOTcfrgkHkEwH6EXDgkd9/YQoSclt96LrSsgg0WJVQztK3m9BQV8i0ig==",
                SecurityStamp = "6Z5465VZLXJGDDY76OG45XRQRBEMHVYP",
                ConcurrencyStamp = "92af299d-be4b-4de7-9b6c-0f43c64ac026"
            };
            IdentityUser guest = new()
            {
                Id = GUEST_ID,
                UserName = "guest@evtinoo.com",
                NormalizedUserName = "GUEST@EVTINOO.COM",
                Email = "guest@evtinoo.com",
                NormalizedEmail = "GUEST@EVTINOO.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEFmQjdr/b3eOTcfrgkHkEwH6EXDgkd9/YQoSclt96LrSsgg0WJVQztK3m9BQV8i0ig==",
                SecurityStamp = "6Z5465VZLXJGDDY76OG45XRQRBEMHVYP",
                ConcurrencyStamp = "92af299d-be4b-4de7-9b6c-0f43c64ac026"
            };

            if (_userManager.FindByIdAsync(USER_ID).Result == null)
            {
                await _userManager.CreateAsync(user);

                await _userManager.AddToRoleAsync(user, "Admin");
            }
            if (_userManager.FindByIdAsync(GUEST_ID).Result == null)
            {
                await _userManager.CreateAsync(guest);

            }

            //var orderss = await context.Orders.ToListAsync();
            //var servicess = await context.Services.ToListAsync();
            //if (servicess.Count == 0)
            //{

            //    await context.Services.AddRangeAsync(services);
            //    await context.SaveChangesAsync(); await context.SaveChangesAsync();

            //}

            //if (orderss.Count == 0)
            //{
            //    await context.Orders.AddRangeAsync(orders);
            //    await context.SaveChangesAsync();

            //}




        }


    }
}
