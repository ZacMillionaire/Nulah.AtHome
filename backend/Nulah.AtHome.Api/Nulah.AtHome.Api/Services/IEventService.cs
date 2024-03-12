using Nulah.AtHome.Data;
using Nulah.AtHome.Data.DTO.Events;

namespace Nulah.AtHome.Api.Services;

public interface IEventService
{
	event EventHandler<List<BasicEventDto>>? EventsUpdated;
	List<BasicEventDto> Events { get; }
	Task LoadEvents(EventListCriteria? listCriteria = null);

	/// <summary>
	/// Creates the event and returns on success
	/// </summary>
	/// <param name="newEvent"></param>
	/// <returns></returns>
	Task<BasicEventDto> CreateEvent(NewBasicEventRequest newEvent);

	Task<BasicEventDto> UpdateEvent(UpdateBasicEventRequest update);
	Task<BasicEventStatsDto> GetStats();
}