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

	[Fact]
	public async void DescriptionRequired()
	{
		var exception = await Assert.ThrowsAsync<Exception>(async () =>
		{
			await TestFixture.CreateBasicEvent(new NewBasicEventRequest()
			{
			});
		});

		Assert.Equal("Description cannot be null", exception.Message);
	}

	[Fact]
	public async void StartRequired()
	{
		var exception = await Assert.ThrowsAsync<Exception>(async () =>
		{
			await TestFixture.CreateBasicEvent(new NewBasicEventRequest()
			{
				Description = "Test Description"
			});
		});

		Assert.Equal("Start date cannot be null", exception.Message);
	}

	[Fact]
	public async void StartDateCannotEqualEndDate()
	{
		var exception = await Assert.ThrowsAsync<Exception>(async () =>
		{
			var now = DateTimeOffset.Now;
			await TestFixture.CreateBasicEvent(new NewBasicEventRequest()
			{
				Description = "Test Description",
				Start = now,
				End = now,
			});
		});

		Assert.Equal("Start date cannot be exactly on or after end date", exception.Message);
	}

	[Fact]
	public async void StartDateCannotExceedEndDate()
	{
		var rand = new Random();

		var exception = await Assert.ThrowsAsync<Exception>(async () =>
		{
			await TestFixture.CreateBasicEvent(new NewBasicEventRequest()
			{
				Description = "Test Description",
				Start = DateTimeOffset.Now.AddDays(1),
				End = DateTimeOffset.Now,
			});
		});

		Assert.Equal("Start date cannot be exactly on or after end date", exception.Message);
	}

	[Theory]
	[MemberData(nameof(CreateBasicEventFixture.EventsToCreate), MemberType = typeof(CreateBasicEventFixture))]
	public async void CreateValidEvents((string description, DateTimeOffset startDate, DateTimeOffset? endDate, List<string>? tags) testData)
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
		("A description", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1), null),
		("A description for longer text", DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.Now.AddDays(1), null),
	};

	public async Task<BasicEventDto> CreateBasicEvent(NewBasicEventRequest basicEventRequest)
	{
		var manager = EventManager;
		using var contextWrapper = DbContextBuilder.Instance.ContextWrapper(manager);

		return await manager.CreateEvent(basicEventRequest);
	}
}