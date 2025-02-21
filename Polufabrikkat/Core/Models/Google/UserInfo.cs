namespace Polufabrikkat.Core.Models.Google
{
	public class UserInfo
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public bool? VerifiedEmail { get; set; }
		public string Name { get; set; }
		public string GivenName { get; set; }
		public string FamilyName { get; set; }
		public string Picture { get; set; }
	}
}
