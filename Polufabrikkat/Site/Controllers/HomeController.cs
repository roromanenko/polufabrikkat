using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Site.Models;
using System.Diagnostics;
using System.Web;

namespace Polufabrikkat.Site.Controllers
{
    [AllowAnonymous]
	public class HomeController : BaseController
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ITikTokService _tikTokService;
		private readonly IUserService _userService;
		private readonly IGoogleService _googleService;

		public HomeController(ILogger<HomeController> logger, ITikTokService tikTokService, IUserService userService, IGoogleService googleService)
		{
			_logger = logger;
			_tikTokService = tikTokService;
			_userService = userService;
			_googleService = googleService;
		}

		public IActionResult Index()
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

		public IActionResult Login(string returnUrl = null)
		{
			var model = new LoginModel
			{
				ReturnUrl = returnUrl
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginModel model)
		{
			var user = await _userService.VerifyUserLogin(model.Username, model.Password);
			if (user != null)
			{
				await LoginUser(user);
				if (!string.IsNullOrEmpty(model.ReturnUrl))
				{
					return Redirect(model.ReturnUrl);
				}
				return RedirectToAction("Index", "Posting");
			}
			model.Error = "Incorrect Username or/and Password";
			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> Register(LoginModel model)
		{
			try
			{
				if (model.Password != model.ConfirmPassword)
				{
					throw new ArgumentException("Password and Confirm Password not equal");
				}

				User user = await _userService.RegisterUser(model.Username, model.Password);
				await LoginUser(user);
				if (!string.IsNullOrEmpty(model.ReturnUrl))
				{
					return Redirect(model.ReturnUrl);
				}
				return RedirectToAction("Index", "Posting");
			}
			catch (ArgumentException ex)
			{
				model.Error = ex.Message;
				return View(nameof(Login), model);
			}
		}

		public async Task<IActionResult> Logout()
		{
			await LogoutUser();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult RedirectToTikTokLogin([FromQuery] string returnUrl, [FromQuery] CallbackStrategy? callbackStrategy)
		{
			var loginUrl = _tikTokService.GetLoginUrl(_tikTokService.GetProcessTikTokLoginResponseUrl(), returnUrl, callbackStrategy ?? CallbackStrategy.Login);
			return Redirect(loginUrl);
		}

		public async Task<IActionResult> ProcessTikTokLoginResponse()
		{
			var response = new TikTokCallbackResponse(Request.Query);

			var tokenData = await _tikTokService.GetAuthToken(HttpUtility.UrlDecode(response.Code), _tikTokService.GetProcessTikTokLoginResponseUrl());
			var userInfo = await _tikTokService.WithAuthData(tokenData).GetUserInfo();

			LoginHandleCallback tikTokHandleCallback = _tikTokService.GetTikTokHandleCallback(response.State);

			if (tikTokHandleCallback == null)
			{
				throw new Exception("Cannot process TikTok login");
			}

			return tikTokHandleCallback.CallbackStrategy switch
			{
				CallbackStrategy.Login => await ExternalLogin(tikTokHandleCallback.ReturnUrl,
					() => _userService.GetUserByTikTokId(userInfo.UnionId),
					() => _userService.UpdateTikTokAuthData(tokenData)),

				CallbackStrategy.AddUser => await AddExternalUser(tikTokHandleCallback.ReturnUrl,
					() => _userService.AddTikTokUser(UserId, new TikTokUser
					{
						AuthTokenData = tokenData,
						UserInfo = userInfo
					})),

				_ => await ExternalLogin(tikTokHandleCallback.ReturnUrl,
					() => _userService.GetUserByTikTokId(userInfo.UnionId),
					() => _userService.UpdateTikTokAuthData(tokenData))
			};
		}

		public IActionResult RedirectToGoogleLogin([FromQuery] string returnUrl, [FromQuery] CallbackStrategy? callbackStrategy)
		{
			var loginUrl = _googleService.GetLoginUrl(returnUrl, callbackStrategy ?? CallbackStrategy.Login);
			return Redirect(loginUrl);
		}

		public async Task<IActionResult> ProcessGoogleLoginResponse()
		{
			var response = new GoogleCallbackResponse(Request.Query);

			var tokenData = await _googleService.GetAuthToken(HttpUtility.UrlDecode(response.Code));
			var userInfo = await _googleService.WithAuthData(tokenData).GetUserInfo();

			LoginHandleCallback googleHandleCallback = _googleService.GetTikTokHandleCallback(response.State);

			if (googleHandleCallback == null)
			{
				throw new Exception("Cannot process Google login");
			}

			return googleHandleCallback.CallbackStrategy switch
			{
				CallbackStrategy.Login => await ExternalLogin(googleHandleCallback.ReturnUrl,
					() => _userService.GetUserByGoogleId(userInfo.Id),
					() => _userService.UpdateGoogleAuthData(tokenData, userInfo.Id)),

				CallbackStrategy.AddUser => await AddExternalUser(googleHandleCallback.ReturnUrl,
					() => _userService.AddGoogleUser(UserId, new Core.Models.Google.GoogleUser
					{
						AuthTokenData = tokenData,
						UserInfo = userInfo
					})),

				_ => await ExternalLogin(googleHandleCallback.ReturnUrl,
					() => _userService.GetUserByGoogleId(userInfo.Id),
					() => _userService.UpdateGoogleAuthData(tokenData, userInfo.Id)),
			};
		}

		private async Task<IActionResult> ExternalLogin(string returnUrl, Func<Task<User>> getUser, Func<Task> refreshData)
		{
			var user = await getUser();
			if (user != null)
			{
				await refreshData();
				await LoginUser(user);

				if (!string.IsNullOrEmpty(returnUrl))
				{
					return Redirect(returnUrl);
				}
				return RedirectToAction("Index", "Posting");
			}

			var model = new LoginModel
			{
				ReturnUrl = returnUrl,
				Error = "You should register and add External user to registered user to be available login via External services"
			};
			return View(nameof(Login), model);
		}

		private async Task<IActionResult> AddExternalUser(string returnUrl, Func<Task> addUser)
		{
			if (!User.Identity.IsAuthenticated)
			{
				var model = new LoginModel
				{
					ReturnUrl = returnUrl
				};
				return View(nameof(Login), model);
			}

			try
			{
				await addUser();
			}
			catch (ArgumentException ex)
			{
				return RedirectToAction("Index", "User", new { Error = ex.Message });
			}

			if (!string.IsNullOrEmpty(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction("Index", "User");
		}
	}
}