using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.Google;
using Polufabrikkat.Core.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Polufabrikkat.Core.ApiClients
{
	public class GoogleApiClient : IGoogleApiClient
	{
		private readonly GoogleApiOptions _options;
		private readonly HttpClient _httpClient;

		public GoogleApiClient(IOptions<GoogleApiOptions> options, HttpClient httpClient)
		{
			_options = options.Value;
			_httpClient = httpClient;
		}

		public string GetLoginUrl(string uniqueIdentificator)
		{
			var responseType = "code";
			var queryString = new Dictionary<string, string>()
			{
				["client_id"] = _options.ClientId,
				["scope"] = _options.Scope,
				["redirect_uri"] = _options.ProcessGoogleLoginResponseUrl,
				["state"] = uniqueIdentificator,
				["response_type"] = responseType
			};

			var authorizationUrl = "https://accounts.google.com/o/oauth2/v2/auth";
			var url = new Uri(QueryHelpers.AddQueryString(authorizationUrl, queryString));

			return url.ToString();
		}

		public async Task<AuthTokenData> GetAuthToken(string code)
		{
			var accessTokenUrl = "https://oauth2.googleapis.com/token";

			var queryString = new Dictionary<string, string>()
			{
				["client_id"] = _options.ClientId,
				["client_secret"] = _options.ClientSecret,
				["code"] = code,
				["grant_type"] = "authorization_code",
				["redirect_uri"] = _options.ProcessGoogleLoginResponseUrl,
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

	}
}
