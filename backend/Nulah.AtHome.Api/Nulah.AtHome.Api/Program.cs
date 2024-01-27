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
			options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.Run();
	}
}