using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RomaPrepod.WebApi
{
	public class Startup
	{
		private const string IndexPath = "/index.html";

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }
		
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
		}
		
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.Use(async (context, next) =>
			{
				await next();
				bool pageNotFound = context.Response.StatusCode == (int)HttpStatusCode.NotFound;
				bool isApiPath = !context.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase);

				if (pageNotFound && isApiPath)
				{
					context.Request.Path = IndexPath;
					await next();
				}
			});

			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseMvc();
		}
	}
}
