using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Models.Google;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IGoogleService
	{
		Task<AuthTokenData> GetAuthToken(string code);
		string GetLoginUrl(string returnUrl, CallbackStrategy callbackStrategy);
		LoginHandleCallback GetTikTokHandleCallback(string state);
		IGoogleAuthenticatedService WithAuthData(AuthTokenData authTokenData);
	}

	public interface IGoogleAuthenticatedService
	{
		Task<UserInfo> GetUserInfo();
	}
}
