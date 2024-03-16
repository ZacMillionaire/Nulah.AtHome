using Nulah.AtHome.Data;
using Nulah.AtHome.Data.DTO.Events;
using Nulah.AtHome.UnitTests.Helpers;

namespace Nulah.AtHome.UnitTests.BasicEvents.Fixtures;

public class CreateBasicEventFixture : DatabaseBackedTestFixture
{
	private EventManager EventManager => CreateEventManager();

	public static TheoryData<(string description, DateTimeOffset startDate, DateTimeOffset? endDate, List<string>? tags)> EventsToCreate() => new()
	{
		(
			"Test Description", DateTimeOffset.Now, null,
			new List<string>() { $"test-{Guid.NewGuid()}", $"test-{Guid.NewGuid()}", $"test-{Guid.NewGuid()}" }
		),
		(
			"A description", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1),
			new List<string>() { $"test-{Guid.NewGuid()}", $"test-{Guid.NewGuid()}", $"test-{Guid.NewGuid()}" }
		),
		(
			"A description for longer text", DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.Now.AddDays(1),
			new List<string>() { $"test-{Guid.NewGuid()}", $"test-{Guid.NewGuid()}", $"test-{Guid.NewGuid()}" }
		),
	};

	public async Task<BasicEventDto> CreateBasicEvent(NewBasicEventRequest basicEventRequest)
	{
		var manager = EventManager;
		// Wrap the manager within our context manager to ensure we don't maintain a hold of tracking entities that could
		// pollute tests with local references instead of retrieving from the database correctly.
		using var contextWrapper = DbContextBuilder.Instance.ContextWrapper(manager);

		return await manager.CreateEvent(basicEventRequest);
	}
}