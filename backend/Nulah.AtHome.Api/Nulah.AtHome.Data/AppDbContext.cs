using Microsoft.EntityFrameworkCore;
using Nulah.AtHome.Data.Converters;
using Nulah.AtHome.Data.Models;
using Nulah.AtHome.Data.Models.Events;

namespace Nulah.AtHome.Data;

public class AppDbContext : DbContext
{
	internal DbSet<Tag> Tags { get; set; }
	internal DbSet<BasicEvent> BasicEvents { get; set; }

	public AppDbContext()
	{
	}

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		//https://github.com/npgsql/npgsql/issues/4176#issuecomment-1064250712
		configurationBuilder
			.Properties<DateTimeOffset>()
			.HaveConversion<DateTimeOffsetConverter>();
	}

	public override int SaveChanges()
	{
		SetCreatedUpdatedForSavingEntities();
		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		SetCreatedUpdatedForSavingEntities();
		return base.SaveChangesAsync(cancellationToken);
	}

	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
	{
		SetCreatedUpdatedForSavingEntities();
		return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	private void SetCreatedUpdatedForSavingEntities()
	{
		var entries = ChangeTracker
			.Entries()
			.Where(e => e.Entity is BaseEntity
			            && (e.State == EntityState.Added
			                || e.State == EntityState.Modified)
			);

		// Entities added/modified in the same batch should have identical timestamps
		var nowUtc = DateTime.UtcNow;

		foreach (var entityEntry in entries)
		{
			((BaseEntity)entityEntry.Entity).UpdatedUtc = nowUtc;

			if (entityEntry.State == EntityState.Added)
			{
				((BaseEntity)entityEntry.Entity).CreatedUtc = nowUtc;
			}
		}
	}

	/// <summary>
	/// Method called when building migrations from command line to create a database in a default location.
	/// <para>
	/// Running the application handles this on its own based on configuration
	/// </para>
	/// </summary>
	/// <param name="options"></param>
	protected override void OnConfiguring(DbContextOptionsBuilder options)
	{
		// If we're called from the cli, configure the data source to be somewhere just so we can build migrations.
		// Any proper context creation should be calling the option builder constructor with its own
		// data source location so this should always be true.
		if (!options.IsConfigured)
		{
			options.UseNpgsql("Host=localhost:55432;Database=Nulah.AtHome;Username=postgres;Password=mysecretpassword",
				x => x.UseNetTopologySuite());
		}

		options.UseSnakeCaseNamingConvention();
	}
}