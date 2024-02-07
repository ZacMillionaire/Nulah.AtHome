using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Nulah.AtHome.Data;
using Nulah.AtHome.Data.DTO.Events;
using Nulah.AtHome.UnitTests.Helpers;
using Xunit.Abstractions;

namespace Nulah.AtHome.UnitTests.BasicEvents;

public class CreateBasicEventTests : TestBase<CreateBasicEventFixture>
{
	public CreateBasicEventTests(CreateBasicEventFixture testFixture, ITestOutputHelper output)
		: base(testFixture, output)
	{
	}

	[Theory]
	[MemberData(nameof(CreateBasicEventFixture.EventsToCreate), MemberType = typeof(CreateBasicEventFixture))]
	public async void CreateValidTests((string description, DateTimeOffset startDate, DateTimeOffset? endDate, List<string>? tags) testData)
	{
		var createdBasicEvent = await TestFixture.CreateBasicEvent(new NewBasicEventRequest()
		{
			Description = testData.description,
			Start = testData.startDate,
			End = testData.endDate,
			Tags = testData.tags
		});

		Assert.NotEqual(0, createdBasicEvent.Id);
		Assert.NotEqual(0u, createdBasicEvent.Version);
		Assert.NotEqual(DateTimeOffset.Now, createdBasicEvent.CreatedUtc);
		Assert.NotEqual(DateTimeOffset.Now, createdBasicEvent.UpdatedUtc);
		Assert.Equal(createdBasicEvent.CreatedUtc, createdBasicEvent.UpdatedUtc);
		Assert.Equal(testData.description, createdBasicEvent.Description);
		Assert.Equal(testData.startDate, createdBasicEvent.Start);
		Assert.Equal(testData.endDate, createdBasicEvent.End);

		if (testData.tags != null)
		{
			// TODO: update the tagdto returned with a comma joined string
			Assert.All(createdBasicEvent.Tags, x =>
			{
				Assert.Contains(x.Name, testData.tags);
				Assert.NotEqual(0, x.Id);
				Assert.NotEqual(0u, x.Version);
				Assert.NotEqual(DateTimeOffset.Now, x.CreatedUtc);
				Assert.NotEqual(DateTimeOffset.Now, x.UpdatedUtc);
			});
		}
	}
}

public class CreateBasicEventFixture : DatabaseBackedTestFixture
{
	private EventManager EventManager => CreateEventManager();

	public static TheoryData<(string description, DateTimeOffset startDate, DateTimeOffset? endDate, List<string>? tags)> EventsToCreate() => new()
	{
		("Test Description", DateTimeOffset.Now, null, null),
		("A description", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1), null)
	};

	public async Task<BasicEventDto> CreateBasicEvent(NewBasicEventRequest basicEventRequest)
	{
		var manager = EventManager;
		using var contextWrapper = DbContextBuilder.Instance.ContextWrapper(manager);

		return await manager.CreateEvent(basicEventRequest);
	}
}