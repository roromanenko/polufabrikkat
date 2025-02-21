using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models;
using Polufabrikkat.Site.Models.User;

namespace Polufabrikkat.Site.Controllers
{
    [Authorize]
	public class UserController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly IUserService _userService;
		private readonly IPostService _postService;
		private readonly ITikTokService _tikTokService;

		public UserController(IMapper mapper, IUserService userService, IPostService postService, ITikTokService tikTokService)
		{
			_mapper = mapper;
			_userService = userService;
			_postService = postService;
			_tikTokService = tikTokService;
		}

		public async Task<IActionResult> Index(UserViewModel model)
		{
			model.AddTikTokUserUrl = Url.Action("RedirectToTikTokLogin", "Home", new { callbackStrategy = CallbackStrategy.AddUser, returnUrl = Url.Action(null, null, null, Request.Scheme) });
			model.AddGoogleUserUrl = Url.Action("RedirectToGoogleLogin", "Home", new { callbackStrategy = CallbackStrategy.AddUser, returnUrl = Url.Action(null, null, null, Request.Scheme) });

			var user = await _userService.GetUserById(UserId);
			model.User = _mapper.Map<UserModel>(user);

			return View(model);
		}

		[HttpDelete]
		public async Task<IActionResult> RemoveTikTokUser(string unionId)
		{
			await _userService.RemoveTikTokUser(UserId, unionId);
			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> ChangePassword([FromBody] NewPasswordRequest request)
		{
			await _userService.ChangePassword(UserId, request.NewPassword);
			return Ok();
		}
	}
}
