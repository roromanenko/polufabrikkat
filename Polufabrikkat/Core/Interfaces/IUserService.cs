using Polufabrikkat.Core.Models.Entities;
using TikTok = Polufabrikkat.Core.Models.TikTok;
using Google = Polufabrikkat.Core.Models.Google;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IUserService
	{
		Task<User> VerifyUserLogin(string username, string password);
		Task<User> RegisterUser(string username, string password);
		Task ChangePassword(string userId, string newPassword);
		Task<User> GetUserById(string userId);
		Task UpdateUser(User user);

		Task<User> GetUserByTikTokId(string unionId);
		Task RemoveTikTokUser(string userId, string tikTokUserUnionId);
		Task AddTikTokUser(string userId, TikTok.TikTokUser tikTokUser);
		Task<TikTok.TikTokUser> GetTikTokUserByUnionId(string unionId);
		Task UpdateTikTokAuthData(TikTok.AuthTokenData authData);

		Task<User> GetUserByGoogleId(string googleId);
		Task UpdateGoogleAuthData(Google.AuthTokenData authData, string googleId);
		Task AddGoogleUser(string userId, Google.GoogleUser googleUser);
	}
}
