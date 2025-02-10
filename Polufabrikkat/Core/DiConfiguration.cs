using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polufabrikkat.Core.ApiClients;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Options;
using Polufabrikkat.Core.Repositories;
using Polufabrikkat.Core.Services;
using Polufabrikkat.Core.Utilities;

namespace Polufabrikkat.Core
{
	public static class DiConfiguration
	{
		public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<ITikTokApiClient, TikTokApiClient>();
			services.AddScoped<ITikTokService, TikTokService>();
			services.AddScoped<IAiGeneratePostService, AiGeneratePostService>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IFileRepository, FileRepository>();
			services.AddScoped<IPostRepository, PostRepository>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IPostService, PostService>();
			services.AddScoped<IOpenAiApiClient, OpenAiApiClient>();
			services.AddScoped<IUnsplashApiClient, UnsplashApiClient>();
			services.AddScoped<IAiImageProcessor, AiImageProcessor>();

			return services;
		}
	}
}
