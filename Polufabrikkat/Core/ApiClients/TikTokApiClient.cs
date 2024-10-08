﻿using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Core.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Polufabrikkat.Core.ApiClients
{
	public class TikTokApiClient : ITikTokApiClient
	{
		private readonly ILogger<TikTokApiClient> _logger;
		private readonly HttpClient _httpClient;
		private readonly TikTokApiOptions _apiOptions;

		public TikTokApiClient(ILogger<TikTokApiClient> logger, HttpClient httpClient, IOptions<TikTokApiOptions> apiOptions)
		{
			_logger = logger;
			_httpClient = httpClient;
			_apiOptions = apiOptions.Value;
		}

		public async Task<AuthTokenData> GetAuthToken(string decodedCode, string redirectUrl)
		{
			var accessTokenUrl = "https://open.tiktokapis.com/v2/oauth/token/";

			var queryString = new Dictionary<string, string>()
			{
				["client_key"] = _apiOptions.ClientKey,
				["client_secret"] = _apiOptions.ClientSecret,
				["code"] = decodedCode,
				["grant_type"] = "authorization_code",
				["redirect_uri"] = redirectUrl,
			};

			using var request = new HttpRequestMessage(HttpMethod.Post, accessTokenUrl)
			{
				Content = new FormUrlEncodedContent(queryString),
			};
			using var res = await _httpClient.SendAsync(request);

			var content = await res.Content.ReadFromJsonAsync<AuthTokenData>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});
			content.RefreshedDate = DateTime.UtcNow;
			return content;
		}

		public string GetLoginUrl(string redirectUrl, string uniqueIdentificator)
		{
			var responseType = "code";
			var queryString = new Dictionary<string, string>()
			{
				["client_key"] = _apiOptions.ClientKey,
				["scope"] = _apiOptions.Scope,
				["redirect_uri"] = redirectUrl,
				["state"] = uniqueIdentificator,
				["response_type"] = responseType
			};

			var authorizationUrl = "https://www.tiktok.com/v2/auth/authorize/";
			var url = new Uri(QueryHelpers.AddQueryString(authorizationUrl, queryString));

			return url.ToString();
		}

		public async Task<UserInfo> GetUserInfo(AuthTokenData authData)
		{
			var getUserInfoUrl = "https://open.tiktokapis.com/v2/user/info/";

			var url = new UriBuilder(getUserInfoUrl);
			url.Query = $"fields={_apiOptions.UserInfoFields}";

			using var request = new HttpRequestMessage(HttpMethod.Get, url.Uri);
			request.Headers.Authorization = new AuthenticationHeaderValue(authData.TokenType, authData.AccessToken);

			using var res = await _httpClient.SendAsync(request);
			var content = await res.Content.ReadFromJsonAsync<UserInfoResponse>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});

			return content.Data.User;
		}

		public async Task<QueryCreatorInfo> GetQueryCreatorInfo(AuthTokenData authData)
		{
			var url = "https://open.tiktokapis.com/v2/post/publish/creator_info/query/";

			using var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authData.TokenType, authData.AccessToken);

			using var res = await _httpClient.SendAsync(request);
			var content = await res.Content.ReadFromJsonAsync<QueryCreatorInfoResponse>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});
			if (content.Error?.Code != "ok")
			{
				throw TikTokApiExceptions.ThrowExceptionFromCode(content.Error.Code);
			}
			content.Data.RefreshedDateTime = DateTime.UtcNow;

			return content.Data;
		}

		public async Task<string> PublishPhotoPost(AuthTokenData authData, PostPhotoRequest postRequest)
		{
			postRequest.MediaType = "PHOTO";
			postRequest.PostMode = "DIRECT_POST";
			postRequest.SourceInfo.Source = "PULL_FROM_URL";

			var url = "https://open.tiktokapis.com/v2/post/publish/content/init/";

			using var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Headers.Authorization = new AuthenticationHeaderValue(authData.TokenType, authData.AccessToken);
			request.Content = JsonContent.Create(postRequest, new MediaTypeHeaderValue("application/json"), new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});
			using var res = await _httpClient.SendAsync(request);

			var content = await res.Content.ReadFromJsonAsync<PostPhotoResponse>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});

			return content.Data.PublishId;
		}

		public async Task<AuthTokenData> RefreshTokenData(AuthTokenData authTokenData)
		{
			var accessTokenUrl = "https://open.tiktokapis.com/v2/oauth/token/";

			var queryString = new Dictionary<string, string>()
			{
				["client_key"] = _apiOptions.ClientKey,
				["client_secret"] = _apiOptions.ClientSecret,
				["grant_type"] = "refresh_token",
				["refresh_token"] = authTokenData.RefreshToken,
			};

			using var request = new HttpRequestMessage(HttpMethod.Post, accessTokenUrl)
			{
				Content = new FormUrlEncodedContent(queryString),
			};
			using var res = await _httpClient.SendAsync(request);

			var content = await res.Content.ReadFromJsonAsync<AuthTokenData>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});
			content.RefreshedDate = DateTime.UtcNow;
			return content;
		}

		public async Task<PostStatusData> GetPostStatus(AuthTokenData authData, PostStatusRequest request)
		{
			var url = "https://open.tiktokapis.com/v2/post/publish/status/fetch/";

			using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue(authData.TokenType, authData.AccessToken);
			httpRequest.Content = JsonContent.Create(request, new MediaTypeHeaderValue("application/json"), new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});

			using var res = await _httpClient.SendAsync(httpRequest);
			var content = await res.Content.ReadFromJsonAsync<PostStatusResponse>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});
			if (content.Error?.Code != "ok")
			{
				throw TikTokApiExceptions.ThrowExceptionFromCode(content.Error.Code);
			}

			return content.Data;
		}
	}
}
