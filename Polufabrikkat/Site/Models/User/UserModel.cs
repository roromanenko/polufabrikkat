namespace Polufabrikkat.Site.Models.User
{
	public class UserModel
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public List<TikTokUserModel> TikTokUsers { get; set; } = new List<TikTokUserModel>();
		public List<GoogleUserModel> GoogleUsers { get; set; } = new List<GoogleUserModel>();
	}
}
