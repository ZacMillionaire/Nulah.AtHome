using Microsoft.Extensions.Logging;
using Nulah.AtHome.Data;
using Xunit.Abstractions;

namespace Nulah.AtHome.UnitTests.Helpers;

public class DatabaseBackedTestFixture : IDisposable
{
	public bool DataSeeded { get; private set; }

	/// <summary>
	/// This is set before any <see cref="Seed"/> methods are run.
	/// </summary>
	private ITestOutputHelper _outputHelper = null!;

	protected DatabaseBackedTestFixture()
	{
		DbContextBuilder.Instance.EnsureMigrationsApplied();
	}

	/// <summary>
	/// Override this method to seed any data for tests.
	/// <para>
	///	This method will be called once before tests run
	/// </para>
	/// </summary>
	public virtual void Seed()
	{
	}

	/// <summary>
	/// This method ensures the <see cref="Seed"/> method is only called once by
	/// any calling <see cref="TestBase{T}"/>.
	/// </summary>
	internal void SeedRunOnlyOnceGuard(ITestOutputHelper outputHelper)
	{
		if (DataSeeded) return;

		DataSeeded = true;
		_outputHelper = outputHelper;
	}

	protected AppDbContext CreateContext() => DbContextBuilder.Instance.CreateContext();

	private ILogger<T> CreateLogger<T>()
	{
		return LoggerFactory.Create(builder =>
				builder.AddColorConsoleLogger(_outputHelper)
			)
			.CreateLogger<T>();
	}

	internal EventManager CreateEventManager() => new(CreateContext(), CreateLogger<EventManager>());

	// maybe make this abstract and the inheriting fixture handles it
	internal void TearDown()
	{
		using var ctx = CreateContext();
		// TODO: replicate this in the managers

		// Remove any additional entities that were created
		/*
		ctx.Users.RemoveRange(ctx.Users.Where(x => UsersCreated.Select(y => y.Id).Contains(x.Id)));
		ctx.Buildings.RemoveRange(ctx.Buildings.Where(x => BuildingsCreated.Select(y => y.Id).Contains(x.Id)));
		ctx.Vehicles.RemoveRange(ctx.Vehicles.Where(x => VehiclesCreated.Select(y => y.Id).Contains(x.Id)));
		*/

		ctx.SaveChanges();
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		// Cleanup
		TearDown();
	}
}