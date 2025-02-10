namespace Polufabrikkat.Core.ApiClients
{
	public class TikTokApiExceptions
	{
		public static BaseTikTokApiException ThrowExceptionFromCode(string errorCode)
		{
			return errorCode switch
			{
				"access_token_invalid" => new TikTokInvalidTokenException("Invalid access token"),
				_ => new BaseTikTokApiException($"TikTok API error: {errorCode}")
			};
		}
	}

	public class BaseTikTokApiException : Exception
	{
		public BaseTikTokApiException()
		{
		}

		public BaseTikTokApiException(string message) : base(message)
		{
		}

		public BaseTikTokApiException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public class TikTokInvalidTokenException : BaseTikTokApiException
	{
		public TikTokInvalidTokenException()
		{
		}

		public TikTokInvalidTokenException(string message) : base(message)
		{
		}

		public TikTokInvalidTokenException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
