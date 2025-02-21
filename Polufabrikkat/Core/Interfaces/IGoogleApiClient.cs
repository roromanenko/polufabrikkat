using Polufabrikkat.Core.Models.Google;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IGoogleApiClient
	{
		Task<AuthTokenData> GetAuthToken(string code);
		string GetLoginUrl(string uniqueIdentificator);
		Task<UserInfo> GetUserInfo(AuthTokenData tokenData);
	}
}
