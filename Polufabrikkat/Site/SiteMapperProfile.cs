using AutoMapper;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.Google;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Site.Models.Posting;
using Polufabrikkat.Site.Models.User;

namespace Polufabrikkat.Site
{
    public class SiteMapperProfile : Profile
	{
		public SiteMapperProfile()
		{
			CreateMap<User, UserModel>();

			CreateMap<TikTokUser, TikTokUserModel>()
				.ForMember(x => x.DisplayName, act => act.MapFrom(scr => scr.UserInfo.DisplayName))
				.ForMember(x => x.UnionId, act => act.MapFrom(scr => scr.UserInfo.UnionId));

			CreateMap<QueryCreatorInfo, QueryCreatorInfoModel>();

			CreateMap<Post, PostShortInto>()
				.ForMember(x => x.Title, act => act.MapFrom(scr => scr.TikTokPostInfo.Title))
				.ForMember(x => x.Description, act => act.MapFrom(scr => scr.TikTokPostInfo.Description));

			CreateMap<TikTokPostInfo, TikTokPostInfoModel>();

			CreateMap<Post, PostModel>();

			CreateMap<PostStatusData, PostStatusDataModel>();

			CreateMap<GoogleUser, GoogleUserModel>()
				.ForMember(x => x.Id, act => act.MapFrom(src => src.UserInfo.Id))
				.ForMember(x => x.FullName, act => act.MapFrom(src => src.UserInfo.Name))
				.ForMember(x => x.Email, act => act.MapFrom(src => src.UserInfo.Email));
		}
	}
}
