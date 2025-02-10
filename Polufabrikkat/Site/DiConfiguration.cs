using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Polufabrikkat.Core.Options;
using Polufabrikkat.Site.Interfaces;
using Polufabrikkat.Site.Options;
using Polufabrikkat.Site.Services;
using SixLabors.ImageSharp;
using System;

namespace Polufabrikkat.Site
{
	public static class DiConfiguration
	{
		private const string SpecificOrigins = "_myAllowSpecificOrigins";

		public static IServiceCollection AddSiteServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
		{
			services.AddControllersWithViews();
			services.AddCors(options =>
			{
				options.AddPolicy(name: SpecificOrigins,
					policy =>
					{
						policy.AllowAnyOrigin()
							.AllowAnyMethod()
							.AllowAnyHeader();
					});
			});
			services.AddHttpClient();
			services.AddMemoryCache();
			services.AddAutoMapper(typeof(SiteMapperProfile));

			services.Configure<FileUploadOptions>(x =>
			{
				x.FileUploadPath = Path.Combine(environment.ContentRootPath, "Images");
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Home/Login";
					options.LogoutPath = "/Home/Logout";
				});

			services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));
			services.Configure<TikTokOptions>(configuration.GetSection(nameof(TikTokOptions)));

			if (environment.IsDevelopment())
			{
				SetupLocalOptions(services, configuration);
			}
			else
			{
                SetupProdOptions(services, configuration);
            }

			services.AddHttpContextAccessor();
			services.AddScoped<IDateTimeProvider, DateTimeProvider>();

			return services;
		}

		public static WebApplication UseSiteServices(this WebApplication app, IWebHostEnvironment environment)
		{
			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			var imagesPath = Path.Combine(environment.ContentRootPath, "Images");
			if (!Directory.Exists(imagesPath))
			{
				Directory.CreateDirectory(imagesPath);
			}
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(imagesPath),
				RequestPath = "/Images"
			});

			app.UseRouting();
			app.UseCors(SpecificOrigins);
			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			return app;
		}


		private static void SetupLocalOptions(IServiceCollection services, IConfiguration configuration)
		{
            services.Configure<TikTokApiOptions>(config =>
            {
                var pubConfig = configuration.GetSection(nameof(TikTokApiOptions)).Get<TikTokApiOptions>();
                config.Scope = pubConfig.Scope;
                config.UserInfoFields = pubConfig.UserInfoFields;
                config.ClientKey = configuration["Polufabrikkat:TikTokClientKey"];
                config.ClientSecret = configuration["Polufabrikkat:TikTokClientSecret"];
            });

            services.AddScoped(opt =>
            {
                string connectionString = configuration["Polufabrikkat:MongodbConnection"];

                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                var client = new MongoClient(settings);

                return client;
            });

			services.Configure<OpenAiApiOptions>(config =>
			{
				config.ApiKey = configuration["Polufabrikkat:OpenAiApiKey"];
            });

            services.Configure<UnsplashApiOptions>(config =>
            {
                config.ApiKey = configuration["Polufabrikkat:UnsplashApiKey"];
            });
        }

        private static void SetupProdOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TikTokApiOptions>(config =>
            {
                var pubConfig = configuration.GetSection(nameof(TikTokApiOptions)).Get<TikTokApiOptions>();
                config.Scope = pubConfig.Scope;
                config.UserInfoFields = pubConfig.UserInfoFields;
                config.ClientKey = Environment.GetEnvironmentVariable("TIKTOK_CLIENT_KEY");
                config.ClientSecret = Environment.GetEnvironmentVariable("TIKTOK_CLIENT_SECRET");
            });

            services.AddScoped(opt =>
            {
                var connectionString = Environment.GetEnvironmentVariable("MONGODB_STRING");

                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                var client = new MongoClient(settings);

                return client;
            });

            services.Configure<OpenAiApiOptions>(config =>
            {
                config.ApiKey = Environment.GetEnvironmentVariable("OPEN_API_KEY");
            });

            services.Configure<UnsplashApiOptions>(config =>
            {
                config.ApiKey = Environment.GetEnvironmentVariable("UNSPLASH_API_KEY");
            });
        }
    }
}
