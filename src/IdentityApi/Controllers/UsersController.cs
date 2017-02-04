using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using IdentityApi.Services;
using IdentityApi.Models;

namespace IdentityApi.Controllers
{
    /// <summary>
    /// Testing Web API controller
    /// </summary>
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;

        public UsersController(IUserService userService) {
            this._userService = userService;
        }


        /// <summary>
        /// Returns list of all values
        /// </summary>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userService.GetAllUsersAsync().Result;
        }

        /// <summary>
        /// Returns one value by its ID
        /// </summary>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Adds new value
        /// </summary>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        /// Updates some value by its ID
        /// </summary>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
