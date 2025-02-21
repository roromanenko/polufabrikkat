using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Models.Google;
using Polufabrikkat.Core.Options;

namespace Polufabrikkat.Core.Services
{
	public class GoogleService : IGoogleService
	{
		private readonly GoogleApiOptions _options;
		private readonly IMemoryCache _memoryCache;
		private readonly IGoogleApiClient _apiClient;

		public GoogleService(IOptions<GoogleApiOptions> options, IMemoryCache memoryCache, IGoogleApiClient apiClient)
		{
			_options = options.Value;
			_memoryCache = memoryCache;
			_apiClient = apiClient;
		}

		public string GetLoginUrl(string returnUrl, CallbackStrategy callbackStrategy)
		{
			var uniqueIdentificatorState = Guid.NewGuid().ToString("N");
			_memoryCache.Set(uniqueIdentificatorState, new LoginHandleCallback
			{
				CallbackStrategy = callbackStrategy,
				ReturnUrl = returnUrl,
			}, _options.LoginCacheInfoTime);

			return _apiClient.GetLoginUrl(uniqueIdentificatorState);
		}

		public Task<AuthTokenData> GetAuthToken(string code)
		{
			return _apiClient.GetAuthToken(code);
		}

		public LoginHandleCallback GetTikTokHandleCallback(string state)
		{
			LoginHandleCallback tikTokHandleCallback = null;
			if (!string.IsNullOrEmpty(state))
			{
				if (_memoryCache.TryGetValue(state, out tikTokHandleCallback))
				{
					_memoryCache.Remove(state);
				}
			}

			return tikTokHandleCallback;
		}

		public IGoogleAuthenticatedService WithAuthData(AuthTokenData authTokenData)
		{
			return new GoogleAuthenticatedService(authTokenData, _apiClient);
		}
	}

	public class GoogleAuthenticatedService : IGoogleAuthenticatedService
	{
		private AuthTokenData _tokenData;
		private readonly IGoogleApiClient _apiClient;

		public GoogleAuthenticatedService(AuthTokenData tokenData, IGoogleApiClient apiClient)
		{
			_tokenData = tokenData;
			_apiClient = apiClient;
		}

		public Task<UserInfo> GetUserInfo()
		{
			return _apiClient.GetUserInfo(_tokenData);
		}
	}
}
