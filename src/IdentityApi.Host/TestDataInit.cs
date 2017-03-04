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
                    FirstName = "Main",
                    LastName = "Admin",
                    Roles = "admin,user"
                };

                await _userManager.CreateAsync(user);

                await _userManager.AddPasswordAsync(user, "admin01");
            }

            user = await _userManager.FindByNameAsync("test01@zumo.org");
            if (user == null) {
                user = new User {
                    Email = "test01@zumo.org",
                    FirstName = "Test",
                    LastName = "User 01",
                    Roles = "user"
                };

                await _userManager.CreateAsync(user, "test01");
            }
        }
    }
}
