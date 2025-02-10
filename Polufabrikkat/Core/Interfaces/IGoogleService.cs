using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Models.Google;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IGoogleService
	{
		Task<AuthTokenData> GetAuthToken(string code);
		string GetLoginUrl(string returnUrl, CallbackStrategy callbackStrategy);
	}
}
