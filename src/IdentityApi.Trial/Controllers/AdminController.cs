using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IdentityApi.Services;

namespace IdentityApi.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IUserService _userService;

        public AdminController(IUserService userService) {
            this._userService = userService;
        }

        public IActionResult Users()
        {
            var users = _userService.GetAllUsersAsync().Result;

            return View(users);
        }

    }
}
