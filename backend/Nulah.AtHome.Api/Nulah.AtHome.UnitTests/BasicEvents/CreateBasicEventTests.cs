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

	[Fact]
	public async void CreateEventWithDuplicateTags()
	{
		var tagList = new List<string>()
		{
			$"test-{Guid.NewGuid()}",
			$"test-{Guid.NewGuid()}",
			$"test-{Guid.NewGuid()}"
		};

		var firstEventRequest = new NewBasicEventRequest()
		{
			Description = $"First Test Description {Guid.NewGuid()}",
			Start = DateTimeOffset.Now,
			Tags = tagList
		};

		var firstCreatedBasicEvent = await TestFixture.CreateBasicEvent(firstEventRequest);

		AssertEventDetails(firstEventRequest, firstCreatedBasicEvent);

		Assert.All(firstCreatedBasicEvent.Tags, x =>
		{
			Assert.Contains(x.Name, tagList);
			Assert.NotEqual(0, x.Id);
			Assert.NotEqual(0u, x.Version);
		});

		var secondEventRequest = new NewBasicEventRequest()
		{
			Description = $"Second Test Description {Guid.NewGuid()}",
			Start = DateTimeOffset.Now.AddDays(1),
			Tags = tagList
		};

		var secondCreatedBasicEvent = await TestFixture.CreateBasicEvent(secondEventRequest);

		AssertEventDetails(secondEventRequest, secondCreatedBasicEvent);

		Assert.All(secondCreatedBasicEvent.Tags, x =>
		{
			Assert.Contains(x.Name, tagList);
			Assert.NotEqual(0, x.Id);
			Assert.NotEqual(0u, x.Version);
		});

		Assert.All(secondCreatedBasicEvent.Tags, x =>
		{
			Assert.NotNull(firstCreatedBasicEvent.Tags.FirstOrDefault(y =>
				y.Name == x.Name
				&& y.Id == x.Id
				&& y.Version == x.Version
			));
		});
	}

	[Theory]
	[MemberData(nameof(CreateBasicEventFixture.EventsToCreate), MemberType = typeof(CreateBasicEventFixture))]
	public async void CreateValidEvents((string description, DateTimeOffset startDate, DateTimeOffset? endDate, List<string>? tags) testData)
	{
		var createEventRequest = new NewBasicEventRequest()
		{
			Description = testData.description,
			Start = testData.startDate,
			End = testData.endDate,
			Tags = testData.tags
		};
		var createdBasicEvent = await TestFixture.CreateBasicEvent(createEventRequest);

		AssertEventDetails(createEventRequest, createdBasicEvent);

		if (testData.tags != null)
		{
			// TODO: update the tagdto returned with a comma joined string
			Assert.All(createdBasicEvent.Tags, x =>
			{
				Assert.Contains(x.Name, testData.tags);
				Assert.NotEqual(0, x.Id);
				Assert.NotEqual(0u, x.Version);
			});
		}
	}

	private void AssertEventDetails(NewBasicEventRequest eventCreationRequest, BasicEventDto createdEvent)
	{
		Assert.NotEqual(0, createdEvent.Id);
		Assert.NotEqual(0u, createdEvent.Version);
		Assert.NotEqual(DateTimeOffset.Now, createdEvent.CreatedUtc);
		Assert.NotEqual(DateTimeOffset.Now, createdEvent.UpdatedUtc);
		Assert.Equal(createdEvent.CreatedUtc, createdEvent.UpdatedUtc);
		Assert.Equal(eventCreationRequest.Description, createdEvent.Description);
		Assert.Equal(eventCreationRequest.Start, createdEvent.Start);
		Assert.Equal(eventCreationRequest.End, createdEvent.End);
	}
}

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
		using var contextWrapper = DbContextBuilder.Instance.ContextWrapper(manager);

		return await manager.CreateEvent(basicEventRequest);
	}
}