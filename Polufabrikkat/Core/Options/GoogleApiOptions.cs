namespace Polufabrikkat.Core.Options
{
	public class GoogleApiOptions
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string ProcessGoogleLoginResponseUrl { get; set; }
		public string Scope { get; set; }
		public TimeSpan LoginCacheInfoTime { get; set; } = TimeSpan.FromMinutes(2);
	}
}
