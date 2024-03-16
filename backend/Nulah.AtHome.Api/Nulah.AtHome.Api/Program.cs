using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nulah.AtHome.Api.Components;
using Nulah.AtHome.Api.Models;
using Nulah.AtHome.Api.Services;
using Nulah.AtHome.Data;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Nulah.AtHome.Api;

public class Program
{
	internal const string ServiceName = "Nulah.AtHome.Api";
	internal const string ServiceVersion = "2024.0.0";

	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Configuration.AddEnvironmentVariables(prefix: "NulahAtHome_");

		AppSettingsPrecheck(builder);

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

		builder.Services.AddDbContext<AppDbContext>((serviceProvider, dbContextOptions) =>
			{
				var appsettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
				//docker run --name some-postgis -p 55432:5432 -e POSTGRES_PASSWORD=mysecretpassword -d postgis/postgis
				dbContextOptions.UseNpgsql(appsettings.Value.ConnectionStrings.Postgres,
					// required to have postgis functionality
					x => x.UseNetTopologySuite());
			}
		);

		AddComponentServices(builder);
		AddDependencies(builder);
		AddOpenTelemetry(builder);

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

	private static void AppSettingsPrecheck(WebApplicationBuilder builder)
	{
		// Check that if we have a value for Seq, it can create a valid Uri object
		if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("Seq"))
		    && !Uri.TryCreate(builder.Configuration.GetConnectionString("Seq"), UriKind.Absolute, out _))
		{
			throw new Exception("SEQ configuration is not empty and not a valid address");
		}

		builder.Services.Configure<AppSettings>(builder.Configuration);
	}

	private static void AddDependencies(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<EventManager>();
	}

	private static void AddComponentServices(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IEventService, EventService>();
	}

	private static void AddOpenTelemetry(WebApplicationBuilder builder)
	{
		builder.Services.AddLogging(logging => logging.AddOpenTelemetry(opts =>
		{
			// Some important options to improve data quality
			opts.IncludeScopes = true;
			opts.IncludeFormattedMessage = true;

			opts.SetResourceBuilder(ResourceBuilder.CreateDefault()
					.AddService(serviceName: ServiceName, serviceVersion: ServiceVersion)
					.AddAttributes(new Dictionary<string, object>
					{
						// Add any desired resource attributes here
						["deployment.environment"] = "development"
					})
				)
				//.AddOtlpExporter()
				.AddConsoleExporter();

			if (builder.Configuration.GetConnectionString("Seq") is { } seqEndpoint
			    && !string.IsNullOrWhiteSpace(seqEndpoint))
			{
				opts.AddOtlpExporter(exporter =>
				{
					// The full endpoint path is required here, when using
					// the `HttpProtobuf` protocol option.
					exporter.Endpoint = new Uri(seqEndpoint);
					exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
					// Optional `X-Seq-ApiKey` header for authentication, if required
					//exporter.Headers = "X-Seq-ApiKey=builder.Configuration.GetValue<string>("Logging:Seq:ApiKey")";
				});
			}
		}));

		builder.Logging.AddOpenTelemetry(opts =>
		{
			// Some important options to improve data quality
			opts.IncludeScopes = true;
			opts.IncludeFormattedMessage = true;

			opts.SetResourceBuilder(ResourceBuilder.CreateDefault()
				.AddService(serviceName: ServiceName, serviceVersion: ServiceVersion)
				.AddAttributes(new Dictionary<string, object>
				{
					// Add any desired resource attributes here
					["deployment.environment"] = "development"
				})
			);
			//.AddConsoleExporter()

			if (builder.Configuration.GetConnectionString("Seq") is { } seqEndpoint
			    && !string.IsNullOrWhiteSpace(seqEndpoint))
			{
				opts.AddOtlpExporter(exporter =>
				{
					// The full endpoint path is required here, when using
					// the `HttpProtobuf` protocol option.
					exporter.Endpoint = new Uri(seqEndpoint);
					exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
					// Optional `X-Seq-ApiKey` header for authentication, if required
					//exporter.Headers = "X-Seq-ApiKey=builder.Configuration.GetValue<string>("Logging:Seq:ApiKey")";
				});
			}
		});

		builder.Services.AddOpenTelemetry()
			.ConfigureResource(resource => resource.AddService(ServiceName))
			.WithTracing(tracing =>
			{
				tracing.AddSource(ServiceName)
					.ConfigureResource(r => r.AddService(serviceName: ServiceName, serviceVersion: ServiceVersion))
					.AddAspNetCoreInstrumentation()
					.AddNulahDataInstrumentation()
					.AddConsoleExporter();

				if (builder.Configuration.GetConnectionString("Seq") is { } seqEndpoint
				    && !string.IsNullOrWhiteSpace(seqEndpoint))
				{
					// This doesn't seem to trace correctly with SEQ yet so I'm unsure of if it's set up correctly or if
					// it's due to blazor specific tracing behaviour
					tracing.AddOtlpExporter(opt =>
					{
						opt.Endpoint = new Uri(seqEndpoint);
						opt.Protocol = OtlpExportProtocol.HttpProtobuf;
						//opt.Headers = "X-Seq-ApiKey=builder.Configuration.GetValue<string>("Logging:Seq:ApiKey")";
					});
				}
			})
			.WithMetrics(metrics =>
			{
				metrics.AddAspNetCoreInstrumentation()
					//.AddConsoleExporter()
					.AddOtlpExporter();
			});
	}
}

public static class Telemetry
{
	public static ActivitySource MyActivitySource => new(Program.ServiceName, Program.ServiceVersion);
}