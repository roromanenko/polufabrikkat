using MongoDB.Bson;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
    public interface IUserRepository
	{
		Task<User> CreateUser(User newUser);
		Task<User> GetUserById(ObjectId userId);
		Task<User> GetUserByTikTokId(string unionId);
		Task<User> GetUserByUsername(string username);
		Task UpdateUser(User user);
		Task RemoveTikTokUser(ObjectId userId, string tikTokUserUnionId);
		Task AddTikTokUser(ObjectId userId, TikTokUser tikTokUser);
		Task UpdateAuthData(AuthTokenData authData);
		Task<TikTokUser> GetTikTokUserByUnionId(string unionId);
		Task<QueryCreatorInfo> GetQueryCreatorInfoByOpenId(string openId);
		Task UpdateQueryCreatorInfo(string openId, QueryCreatorInfo queryCreatorInfo);
		Task ChangePassword(ObjectId userId, string newPasswordHash);
		Task<User> GetUserByGoogleId(string googleId);
		Task AddGoogleUser(ObjectId userId, Models.Google.GoogleUser googleUser);
		Task UpdateGoogleAuthData(Models.Google.AuthTokenData authData, string googleId);
	}
}
