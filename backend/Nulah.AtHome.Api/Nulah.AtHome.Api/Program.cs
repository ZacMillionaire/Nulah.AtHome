using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Nulah.AtHome.Data;

namespace Nulah.AtHome.Api;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddAuthorization();

		builder.Services.AddControllers().AddJsonOptions(options =>
		{
			// Ensure properties are output in the casing they have in code without being forced to camelCase or otherwise
			// Property is returned as Property
			// someWeirdProperty is returned as someWeirdProperty
			options.JsonSerializerOptions.PropertyNamingPolicy = null;
			options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
		});

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddDbContext<AppDbContext>(dbContextOptions =>
			{
				//docker run --name some-postgis -p 55432:5432 -e POSTGRES_PASSWORD=mysecretpassword -d postgis/postgis
				dbContextOptions.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"),
					// required to have postgis functionality
					x => x.UseNetTopologySuite());
			}
		);

		builder.Services.AddTransient<EventManager>();

		// If we're in development mode, the frontend will be running externally, so we'll need CORS
		if (builder.Environment.IsDevelopment())
		{
			builder.Services.AddCors(options =>
			{
				options.AddPolicy(name: builder.Configuration.GetSection("CORS:PolicyName").Value,
					policy =>
					{
						policy.WithOrigins(builder.Configuration.GetSection("Api:FrontEndDomain").Value);
						policy.AllowCredentials();
						policy.AllowAnyHeader();
						policy.WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Patch);
					});
			});
		}

		var app = builder.Build();

#if DEBUG
		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			// Only run this during development to make sure the database is using latest migrations each run
			using (var serviceScope = app.Services.CreateScope())
			{
				var ctx = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

				// Ensure migrations are applied so any sql scripts are run
				ctx.Database.Migrate();
				// Then ensure that it actually created
				ctx.Database.EnsureCreated();
			}

			app.UseSwagger();
			app.UseSwaggerUI();
			// If we're in development mode, the frontend will be running externally, so we'll need CORS
			app.UseCors(builder.Configuration.GetSection("CORS:PolicyName").Value);
		}
#endif

#if DEBUG
		app.UseHttpsRedirection();
#endif

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapDefaultControllerRoute()
			.RequireAuthorization()
			.WithOpenApi();

		// Only use files from wwwroot with fallback if we're running in non-development
		// if (!app.Environment.IsDevelopment())
		// {
		app.UseDefaultFiles();
		app.UseStaticFiles();

		// required for angular spa fall back
		// annoying bug: routes such as /events won't correctly show the events page if it's a direct navigation
		// by url
		app.MapFallbackToFile("/index.html");
		// }

		app.Run();
	}
}