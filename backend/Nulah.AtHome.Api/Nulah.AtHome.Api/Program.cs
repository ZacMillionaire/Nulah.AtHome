using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Nulah.AtHome.Api.Components;
using Nulah.AtHome.Api.Services;
using Nulah.AtHome.Data;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Nulah.AtHome.Api;

public class Program
{
	internal const string ServiceName = "Nulah.AtHome.Api";

	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();

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

		builder.Services.AddScoped<EventService>();

		builder.Services.AddScoped<EventManager>();


		builder.Logging.AddOpenTelemetry(opts =>
		{
			opts.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName))
				.AddOtlpExporter()
				.AddConsoleExporter();
		});

		builder.Services.AddOpenTelemetry()
			.ConfigureResource(resource => resource.AddService(ServiceName))
			.WithTracing(tracing =>
			{
				tracing.AddSource(ServiceName)
					.ConfigureResource(r => r.AddService(serviceName: ServiceName, serviceVersion: "0.0.1"))
					.AddAspNetCoreInstrumentation()
					.AddNulahDataInstrumentation()
					.AddConsoleExporter()
					.AddOtlpExporter();
			})
			.WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation().AddConsoleExporter().AddOtlpExporter());

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
		}
#endif

#if DEBUG
		app.UseHttpsRedirection();
#endif


		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();
		app.UseStaticFiles();
		app.UseAntiforgery();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.MapDefaultControllerRoute()
			.RequireAuthorization()
			.WithOpenApi();

		app.Run();
	}
}

public static class Telemetry
{
	public static ActivitySource MyActivitySource => new(Program.ServiceName, "0.0.1");
}