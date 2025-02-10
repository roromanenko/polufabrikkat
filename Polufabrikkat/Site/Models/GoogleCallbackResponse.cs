namespace Polufabrikkat.Site.Models
{
	public class GoogleCallbackResponse
	{
		public string Code { get; set; }
		public string State { get; set; }
		public string Scope { get; set; }

		public GoogleCallbackResponse()
		{
		}
		public GoogleCallbackResponse(IQueryCollection query)
		{
			Code = query["code"];
			Scope = query["scope"];
			State = query["state"];
		}
	}
}
