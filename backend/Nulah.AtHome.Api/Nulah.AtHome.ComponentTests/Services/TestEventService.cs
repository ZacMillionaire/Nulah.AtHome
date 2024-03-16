using Nulah.AtHome.Api.Services;
using Nulah.AtHome.Data.Criteria;
using Nulah.AtHome.Data.DTO;
using Nulah.AtHome.Data.DTO.Events;

namespace Nulah.AtHome.ComponentTests.Services;

internal class TestEventService : IEventService
{
	public event EventHandler<List<BasicEventDto>>? EventsUpdated;

	public List<BasicEventDto> Events { get; } = new();

	private readonly TimeProvider _timeProvider;

	public TestEventService(TimeProvider timeProvider)
	{
		_timeProvider = timeProvider;
	}

	public async Task LoadEvents(EventListCriteria? listCriteria = null)
	{
		await Task.Delay(TimeSpan.FromSeconds(1), _timeProvider);
	}

	public async Task<BasicEventDto> CreateEvent(NewBasicEventRequest newEvent)
	{
		var newEventDto = new BasicEventDto()
		{
			Description = newEvent.Description!,
			Start = newEvent.Start!.Value,
			Tags = newEvent.Tags!
				.Select(x => new TagDto()
				{
					Name = x
				})
				.ToList()
		};

		Events.Add(newEventDto);

		EventsUpdated?.Invoke(this, Events);

		await Task.Delay(0);

		return newEventDto;
	}

	public async Task<BasicEventDto> UpdateEvent(UpdateBasicEventRequest update)
	{
		throw new NotImplementedException();
	}

	public async Task<BasicEventStatsDto> GetStats()
	{
		throw new NotImplementedException();
	}
}