using Microsoft.EntityFrameworkCore;
using Nulah.AtHome.Data;

namespace Nulah.AtHome.UnitTests.Helpers;

public sealed class DbContextBuilder
{
	private static DbContextBuilder? _instance;

	/// <summary>
	/// Returns the current instance.
	/// <para>
	/// If called before <see cref="InstanceWithConnnectionString"/>: Sets the connection string using appsettings.Test.json to locate the connection string.
	/// </para>
	/// <para>
	/// If called after <see cref="InstanceWithConnnectionString"/>: returns the instance with the connection string given.
	/// </para>
	/// </summary>
	public static DbContextBuilder Instance
	{
		get
		{
			// Ensure we only get 1 instance by locking to the first thread until done.
			lock (Lock)
			{
				return _instance ??= new();
			}
		}
	}

	/// <summary>
	/// Sets the connection string to the given connection string.
	/// <para>
	///	Repeated calls to this regardless of string will always return the same instance. <see cref="Instance"/> will also return the same.
	/// </para>
	/// <para>
	///	If <see cref="Instance"/> was used before this call, the connection will be set to appsettings.Test.json
	/// </para>
	/// </summary>
	public static readonly Func<string, DbContextBuilder> InstanceWithConnnectionString = connectionString =>
	{
		// Ensure we only get 1 instance by locking to the first thread until done.
		lock (Lock)
		{
			return _instance ??= new(connectionString);
		}
	};

	private bool _contextCreated;
	private readonly string _postgresConnectionString;
	private static readonly object Lock = new();

	public Guid instanceId = Guid.NewGuid();

	private DbContextBuilder()
	{
		//var settings = JsonSerializer.Deserialize<TestAppSettings>(File.ReadAllText("./appsettings.Test.json"))!;
		_postgresConnectionString = "Host=localhost:55432;Database=Nulah.AtHome_Test;Username=postgres;Password=mysecretpassword"; //settings.ConnectionStrings.Sql;
	}

	private DbContextBuilder(string connectionString)
	{
		// not sure if I really need, the default constructor should handle it
		throw new NotImplementedException();
		_postgresConnectionString = connectionString;
	}

	/// <summary>
	/// Call this to ensure the target database exists and has all migrations applied.
	/// </summary>
	public void EnsureMigrationsApplied()
	{
		// Multiple locations might be trying to set the initial state of the context
		// from multiple test threads, so we lock here to ensure all test fixtures/initialisers
		// wait until at least 1 thread has created the context
		lock (Lock)
		{
			// Only create the context once
			if (!_contextCreated)
			{
				// Seed the database in a using to prevent loading the entire seeded database into
				// local memory (and thus avoiding issues with missing Includes)
				using (var ctx = CreateContext())
				{
					ctx.Database.EnsureDeleted();
					// Ensure migrations are applied so any sql scripts are run
					ctx.Database.Migrate();
					// Then ensure that it actually created
					ctx.Database.EnsureCreated();
				}

				_contextCreated = true;
			}
		}
	}

	public AppDbContext CreateContext()
	{
		return new(
			new DbContextOptionsBuilder<AppDbContext>()
				.UseNpgsql(_postgresConnectionString,
					x => x.UseNetTopologySuite())
				.Options
		);
	}

	/// <summary>
	/// Creates a wrapper around a class instance that has a reference to <see cref="AppDbContext"/>
	/// </summary>
	/// <param name="contextConsumingClass"></param>
	/// <returns></returns>
	public TestDbContextWrapper ContextWrapper(object contextConsumingClass) => new(contextConsumingClass);
}