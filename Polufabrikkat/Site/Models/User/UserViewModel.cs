namespace Polufabrikkat.Site.Models.User
{
	public class UserViewModel : BaseModel
	{
		public UserModel User { get; set; }
		public string AddTikTokUserUrl { get; set; }
		public string AddGoogleUserUrl { get; set; }
	}
}
