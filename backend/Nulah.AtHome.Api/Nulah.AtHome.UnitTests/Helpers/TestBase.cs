using Xunit.Abstractions;

namespace Nulah.AtHome.UnitTests.Helpers;

/// <summary>
/// Used to back a test with a fixture
/// </summary>
/// <typeparam name="T"></typeparam>
public class TestBase<T> : IClassFixture<T>
	where T : DatabaseBackedTestFixture
{
	protected readonly T TestFixture;
	protected readonly ITestOutputHelper TestOutput;

	protected TestBase(T testFixture, ITestOutputHelper output)
	{
		TestFixture = testFixture;
		TestOutput = output;

		if (!TestFixture.DataSeeded)
		{
			// Ensure the fixture has the output helper
			TestFixture.SeedRunOnlyOnceGuard(output);
			// run the fixtures seed method
			TestFixture.Seed();
		}
	}
}