using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Polufabrikkat.Models;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Polufabrikkat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
			return View();
        }

		public IActionResult Privacy()
        {
            return View();
        }

		public IActionResult TermsOfService()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		public IActionResult RedirectToTikTokLogin()
		{
			var clientKey = "awcas2aer6q9nym5"; // from tiktok dev
			var scope = "user.info.basic";
			var redirectUrl = Url.Action("ProcessTikTokLoginResponse", "Home", null, Request.Scheme, Request.Host.Value);
			var uniqueIdentificatorState = Guid.NewGuid().ToString("N");
			var responseType = "code";
			var queryString = new Dictionary<string, string>()
			{
				["client_key"] = clientKey,
				["scope"] = scope,
				["redirect_uri"] = redirectUrl,
				["state"] = uniqueIdentificatorState,
				//["code_challenge"] = uniqueIdentificatorState,
				//["code_challenge_method"] = "plain",
				["response_type"] = responseType
			};

			var authorizationUrl = "https://www.tiktok.com/v2/auth/authorize/";
			var url = new Uri(QueryHelpers.AddQueryString(authorizationUrl, queryString));

			return Redirect(url.ToString());
		}

		public IActionResult ProcessTikTokLoginResponse()
		{
			var response = Response;
			return RedirectToAction("Index", "Home");
		}

	}
}