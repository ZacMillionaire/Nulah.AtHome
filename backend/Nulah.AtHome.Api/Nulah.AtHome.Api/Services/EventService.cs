using System.Diagnostics;
using Nulah.AtHome.Data;
using Nulah.AtHome.Data.DTO.Events;
using OpenTelemetry.Trace;

namespace Nulah.AtHome.Api.Services;

internal class EventService : IEventService
{
	public event EventHandler<List<BasicEventDto>>? EventsUpdated;
	public List<BasicEventDto> Events { get; private set; } = new();

	private EventManager EventManager => _serviceCollection.GetRequiredService<EventManager>();
	private readonly IServiceProvider _serviceCollection;

	public EventService(IServiceProvider services)
	{
		_serviceCollection = services;
	}

	public async Task LoadEvents(EventListCriteria? listCriteria = null)
	{
		using var updateActivity = Telemetry.MyActivitySource.StartActivity(ActivityKind.Internal);
		Events = await EventManager.GetEvents(listCriteria);
		EventsUpdated?.Invoke(this, Events);
	}

	/// <summary>
	/// Creates the event and returns on success
	/// </summary>
	/// <param name="newEvent"></param>
	/// <returns></returns>
	public async Task<BasicEventDto> CreateEvent(NewBasicEventRequest newEvent)
	{
		using var createActivity = Telemetry.MyActivitySource.StartActivity(ActivityKind.Internal);

		try
		{
			var createdEvent = await EventManager.CreateEvent(newEvent);

			createActivity?.AddEvent(new ActivityEvent("New event created",
					DateTimeOffset.Now,
					new(
						new Dictionary<string, object?>
						{
							{ "event.id", createdEvent.Id }
						}
					)
				)
			);

			return createdEvent;
		}
		catch (Exception ex)
		{
			// trace the exception then throw it back to the component
			createActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			createActivity?.RecordException(ex);

			throw;
		}
	}

	public async Task<BasicEventDto> UpdateEvent(UpdateBasicEventRequest update)
	{
		using var updateActivity = Telemetry.MyActivitySource.StartActivity(ActivityKind.Internal);
		updateActivity?.SetTag("event.id", update.Id);
		updateActivity?.SetTag("event.version", update.Version);

		try
		{
			var updatedEvent = await EventManager.UpdateEvent(update);

			if (update.Version != updatedEvent.Version)
			{
				var eventTags = new Dictionary<string, object?>
				{
					{ "event.version", update.Version }
				};

				updateActivity?.AddEvent(new ActivityEvent("Event Version Change", DateTimeOffset.Now, new(eventTags)));
			}
			else
			{
				updateActivity?.AddEvent(new ActivityEvent("Event updated without differences", DateTimeOffset.Now));
			}

			return updatedEvent;
		}
		catch (Exception ex)
		{
			// trace the exception then throw it back to the component
			updateActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			updateActivity?.RecordException(ex);

			throw;
		}
	}

	public async Task<BasicEventStatsDto> GetStats()
	{
		return await EventManager.GetStats();
	}
}