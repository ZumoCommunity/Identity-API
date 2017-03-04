using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


using IdentityApi.Services;
using IdentityApi.Models;

using IdentityApi.Models.UserViewModels;


namespace Gazda.Web.Admin.Controllers
{

    [Authorize(Roles = "admin")]
    public class UserController : Controller {

        private readonly IUserService userService;
        private readonly UserManager<User> _userManager;


        public UserController(UserManager<User> userManager, IUserService userService) {
            this._userManager = userManager;
            this.userService = userService;
        }

        public async Task<IActionResult> Index(){
            var model = await userService.GetAllUsersAsync();
            return View("Users", model);
        }

        public async Task<IActionResult> Details(string id){
            var model = await _userManager.FindByIdAsync(id);
            return View("UserDetails", model);
        }

        public async Task<IActionResult> Edit(string id) {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                //!!!!!!!!!!!!!!!! Add error to flash data
                return RedirectToAction("Index");
            }
            var uvm = new UserViewModel(user);
            return View("UserEdit", uvm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User uvm){
            if (ModelState.IsValid) {
                var user = await _userManager.FindByIdAsync(uvm.Id);

                user.FirstName = uvm.FirstName;
                user.LastName = uvm.LastName;

                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return View("UserEdit", uvm);
        }

        //public async Task<IActionResult> Add() {
        //    var model = new UserViewModel();
        //    return View("UserAdd", model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Add(UserViewModel uvm) {
        //    string adminId = this.User.GetUserId();

        //    if (ModelState.IsValid) {
        //        var newUser = new User();
        //        uvm.CopyTo(newUser);
        //        //newUser.PasswordHash = userService.HashPassword(newUser, uvm.Email);
        //        //newUser.UserRoles = "CONTENT";
        //        var result = await _userManager.CreateAsync(newUser);
        //        if (result != IdentityResult.Success) {
        //            foreach (var error in result.Errors) {
        //                ModelState.AddModelError(error.Code, error.Description);
        //            }
        //        }
        //        else
        //            return RedirectToAction("UserDetails", new { id = newUser.Id });
        //    }

        //    return View("UserAdd", uvm);
        //}

    }
}

