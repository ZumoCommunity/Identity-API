using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApi.Models;


namespace IdentityApi
{
    public interface ITestDataInitializer {
        Task InitTestDataAsync();
    }

    public class TestDataInitializer : ITestDataInitializer
    {
        private UserManager<User> _userManager;

        public TestDataInitializer(UserManager<User> userManager) {
            this._userManager = userManager;

        }


        public async Task InitTestDataAsync() {
            var user = await _userManager.FindByNameAsync("admin@zumo.org");
            if (user == null) {
                user = new User {
                    Email = "admin@zumo.org",
                    FullName = "Admin User 01",
                    Roles = "admin,user"
                };

                await _userManager.CreateAsync(user);

                await _userManager.AddPasswordAsync(user, "admin01");
            }

            user = await _userManager.FindByNameAsync("test02@zumo.org");
            if (user == null) {
                user = new User {
                    Email = "test02@zumo.org",
                    FullName = "Test User 02",
                    Roles = "user"
                };

                await _userManager.CreateAsync(user, "test02");
            }
        }
    }
}
