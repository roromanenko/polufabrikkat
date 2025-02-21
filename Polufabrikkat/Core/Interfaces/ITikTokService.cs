using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
	public interface ITikTokService
	{
		Task<AuthTokenData> GetAuthToken(string code, string tiktokCallbackUrl);
		string GetLoginUrl(string tiktokCallbackUrl, string returnUrl, CallbackStrategy callbackStrategy);
		LoginHandleCallback GetTikTokHandleCallback(string state);
		string GetProcessTikTokLoginResponseUrl();

		ITikTokAuthenticatedService WithAuthData(AuthTokenData authTokenData);
	}

	public interface ITikTokAuthenticatedService
	{
		Task<QueryCreatorInfo> GetQueryCreatorInfo();
		Task<UserInfo> GetUserInfo();
		Task PublishPhotoPost(Post newPost);
		Task<PostStatusData> RefreshTikTokPostStatus(string postId, string publishId);
	}
}
