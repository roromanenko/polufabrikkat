namespace Polufabrikkat.Core.Models
{
	public class LoginHandleCallback
	{
		public string ReturnUrl { get; set; }
		public CallbackStrategy CallbackStrategy { get; set; }
	}

	public enum CallbackStrategy
	{
		Login,
		AddUser
	}
}
