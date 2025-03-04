﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Site.Helpers;
using Polufabrikkat.Site.Interfaces;
using Polufabrikkat.Site.Models.Posting;
using Polufabrikkat.Site.Models.User;
using Polufabrikkat.Site.Options;

namespace Polufabrikkat.Site.Controllers
{
	[Authorize]
	public class PostingController : BaseController
	{
		private readonly FileUploadOptions _fileUploadOptions;
		private readonly IUserService _userService;
		private readonly ITikTokService _tikTokService;
		private readonly IMapper _mapper;
		private readonly IPostService _postService;
		private readonly IDateTimeProvider _dateTimeProvider;

		public PostingController(IOptions<FileUploadOptions> fileUploadOptions, IUserService userService,
			ITikTokService tikTokService, IMapper mapper, IPostService postService,
			IDateTimeProvider dateTimeProvider)
		{
			_fileUploadOptions = fileUploadOptions.Value;
			_userService = userService;
			_tikTokService = tikTokService;
			_mapper = mapper;
			_postService = postService;
			_dateTimeProvider = dateTimeProvider;
		}

		public async Task<IActionResult> NewPost(NewPostViewModel model)
		{
			var user = await _userService.GetUserById(UserId);
			model.TikTokUsers = _mapper.Map<List<TikTokUserModel>>(user.TikTokUsers);
			return View(model);
		}

		public async Task<IActionResult> Index(PostHistoryViewModel model)
		{
			var posts = await _postService.GetFilteredPosts(userId: UserId);
			model.Posts = posts.Select(x => new ShortPostModel(x, _dateTimeProvider)).ToList();

			return View(model);
		}

		public async Task<IActionResult> Post(PostViewModel model)
		{
			var post = await _postService.GetPostById(model.Id);
			model.Post = _mapper.Map<PostModel>(post);

			model.Post.Created = _dateTimeProvider.ConvertToClienTimezoneFromUtc(model.Post.Created);
			model.Post.ScheduledPublicationTime = _dateTimeProvider.ConvertToClienTimezoneFromUtc(model.Post.ScheduledPublicationTime);

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> SelectTikTokUser(string unionId)
		{
			var tiktokUser = await _userService.GetTikTokUserByUnionId(unionId);
			if (tiktokUser == null)
			{
				return BadRequest(new { error = "TikTok user not found" });
			}

			var queryCreatorInfo = await _tikTokService.WithAuthData(tiktokUser.AuthTokenData).GetQueryCreatorInfo();

			return Json(_mapper.Map<QueryCreatorInfoModel>(queryCreatorInfo));
		}

		[HttpPost]
		public async Task<IActionResult> CreateNewPhotoPost([FromForm] NewPhotoPostRequest request)
		{
			var tiktokUser = await _userService.GetTikTokUserByUnionId(request.TikTokUserUnionId);
			if (tiktokUser == null)
			{
				return BadRequest("Incorrect TikTok user id");
			}
			if (request.Files == null || !request.Files.Any())
			{
				return BadRequest("Files are not selected");
			}
			if (request.PhotoCoverIndex <= 0 || request.PhotoCoverIndex > request.Files.Count)
			{
				return BadRequest("Cover index not correct");
			}

			var filesToUpload = new List<Core.Models.Entities.File>();
			foreach (var file in request.Files)
			{
				if (!PhotoHelper.IsValidFileSize(file.Length))
				{
					return BadRequest("File size exceeds 10 MB limit.");
				}
				if (!PhotoHelper.IsMimeTypeAllowed(file.ContentType))
				{
					return BadRequest("Allowed only WebP and JPEG formats.");
				}
				var fileStream = file.OpenReadStream();
				if (!PhotoHelper.IsValidPictureSize(fileStream))
				{
					return BadRequest("Allows only 1080p picture size.");
				}

				using var memoryStream = new MemoryStream();
				await file.CopyToAsync(memoryStream);
				var newFile = new Core.Models.Entities.File
				{
					Added = DateTime.UtcNow,
					ContentType = file.ContentType,
					FileName = Guid.NewGuid().ToString() + PhotoHelper.GetExtensionFromMimeType(file.ContentType),
					FileData = memoryStream.ToArray()
				};
				filesToUpload.Add(newFile);
			}

			var newPost = new Core.Models.Entities.Post
			{
				UserId = ObjectId.Parse(UserId),
				TikTokUserUnionId = request.TikTokUserUnionId,
				TikTokPostInfo = new Core.Models.Entities.TikTokPostInfo
				{
					AutoAddMusic = request.AutoAddMusic,
					Description = request.Description,
					DisableComment = request.DisableComment,
					PhotoCoverIndex = request.PhotoCoverIndex - 1,
					PrivacyLevel = request.PrivacyLevel,
					Title = request.Title
				},
				Type = Core.Models.Entities.PostType.Photo,
				Status = Core.Models.Entities.PostStatus.Created,
				Created = DateTime.UtcNow,
				ScheduledPublicationTime = _dateTimeProvider.ConvertToUtcFromClientTimezone(request.ScheduledPublicationTime),
			};

			newPost = await _postService.AddNewPost(newPost, filesToUpload);

			if (newPost.ScheduledPublicationTime == null)
			{
				await _tikTokService.WithAuthData(tiktokUser.AuthTokenData).PublishPhotoPost(newPost);
			}
			return Json(newPost.Id.ToString());
		}

		[HttpPost]
		public async Task<IActionResult> RefreshTikTokPostStatus([FromBody] RefreshTikTokPublicationStatusRequest request)
		{
			var tikTokUser = await _userService.GetTikTokUserByUnionId(request.TikTokUserUnionId);
			var result = await _tikTokService.WithAuthData(tikTokUser.AuthTokenData).RefreshTikTokPostStatus(request.PostId, request.PublicationId);


			return Json(_mapper.Map<PostStatusDataModel>(result));
		}
	}
}
