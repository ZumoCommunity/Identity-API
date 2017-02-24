using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using IdentityModel.Client;

using MvcClient.Options;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private EndpointOptions _endpointOptions;

        public HomeController(IOptions<EndpointOptions> optionsAccessor) {
            this._endpointOptions = optionsAccessor.Value;
        }


        public IActionResult Index() {
            return View();
        }

        [Authorize]
        public IActionResult UserProfile() {

            return View();
        }

        public async Task Logout() {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            await HttpContext.Authentication.SignOutAsync("oidc");
        }

        public IActionResult Error() {
            return View();
        }

        public async Task<IActionResult> CallApiUsingClientCredentials() {
            var authEndpoint = _endpointOptions.IdentityApi;
            var tokenClient = new TokenClient(authEndpoint + "/connect/token", "mvc", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var content = await client.GetStringAsync("http://localhost:5001/identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }

        public async Task<IActionResult> CallApiUsingUserAccessToken() {
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var content = await client.GetStringAsync("http://localhost:5001/identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }
    }
}