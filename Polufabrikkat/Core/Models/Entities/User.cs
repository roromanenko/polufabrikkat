using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Polufabrikkat.Core.Models.Google;
using Polufabrikkat.Core.Models.TikTok;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polufabrikkat.Core.Models.Entities
{
	[Table("users")]
	public class User
	{
		[BsonId]
		public ObjectId Id { get; set; }
		public string Username { get; set; }
		public string PasswordHash { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
		public List<TikTokUser> TikTokUsers { get; set; } = new List<TikTokUser>();
		public List<GoogleUser> GoogleUsers { get; set; } = new List<GoogleUser>();
	}
}
