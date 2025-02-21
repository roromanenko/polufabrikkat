using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Polufabrikkat.Core.Constants;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly PasswordHasher<User> _passwordHasher;

		public UserService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
			_passwordHasher = new PasswordHasher<User>();
		}

		public async Task<User> VerifyUserLogin(string username, string password)
		{
			var user = await _userRepository.GetUserByUsername(username);
			if (user == null)
			{
				return user;
			}

			var result = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, password);

			if (result == PasswordVerificationResult.Success)
			{
				return user;
			}

			return null;
		}

		public async Task<User> RegisterUser(string username, string password)
		{
			var existingUser = await _userRepository.GetUserByUsername(username);
			if (existingUser != null)
			{
				throw new ArgumentException("User with this username is already exists");
			}

			var newUser = new User
			{
				Username = username,
				PasswordHash = _passwordHasher.HashPassword(null, password),
				Roles = new List<string> { AppRoles.User }
			};
			return await _userRepository.CreateUser(newUser);
		}

		public Task ChangePassword(string userId, string newPassword)
		{
			return _userRepository.ChangePassword(ObjectId.Parse(userId), _passwordHasher.HashPassword(null, newPassword));
		}

		public async Task<User> GetUserById(string userId)
		{
			return await _userRepository.GetUserById(ObjectId.Parse(userId));
		}

		public async Task UpdateUser(User user)
		{
			await _userRepository.UpdateUser(user);
		}

		public Task<User> GetUserByTikTokId(string unionId)
		{
			return _userRepository.GetUserByTikTokId(unionId);
		}

		public Task RemoveTikTokUser(string userId, string tikTokUserUnionId)
		{
			return _userRepository.RemoveTikTokUser(ObjectId.Parse(userId), tikTokUserUnionId);
		}

		public Task AddTikTokUser(string userId, TikTokUser tikTokUser)
		{
			return _userRepository.AddTikTokUser(ObjectId.Parse(userId), tikTokUser);
		}

		public Task<TikTokUser> GetTikTokUserByUnionId(string unionId)
		{
			return _userRepository.GetTikTokUserByUnionId(unionId);
		}

		public Task UpdateTikTokAuthData(AuthTokenData authData)
		{
			return _userRepository.UpdateAuthData(authData);
		}


		public Task<User> GetUserByGoogleId(string googleId)
		{
			return _userRepository.GetUserByGoogleId(googleId);
		}

		public Task AddGoogleUser(string userId, Models.Google.GoogleUser googleUser)
		{
			return _userRepository.AddGoogleUser(ObjectId.Parse(userId), googleUser);
		}

		public Task UpdateGoogleAuthData(Models.Google.AuthTokenData authData, string googleId)
		{
			return _userRepository.UpdateGoogleAuthData(authData, googleId);
		}
	}
}
