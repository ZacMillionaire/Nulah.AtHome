using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nulah.AtHome.Data.DTO.Events;
using Nulah.AtHome.Data.Models.Events;

namespace Nulah.AtHome.Data;

public class EventManager
{
	private readonly AppDbContext _context;
	private readonly ILogger _logger;
	private Guid _instanceId = Guid.NewGuid();

	public EventManager(AppDbContext context, ILogger<EventManager> logger)
	{
		_context = context;
		_logger = logger;
		_logger.LogDebug("[{instanceId}] EventManager created", _instanceId);
	}

	public async Task<List<BasicEventDto>> GetEvents()
	{
		return await _context.BasicEvents.Select(x => new BasicEventDto()
			{
				Description = x.Description,
				End = x.End,
				Id = x.Id,
				Start = x.Start,
				//Tags =
				Version = x.Version,
				CreatedUtc = x.CreatedUtc,
				UpdatedUtc = x.UpdatedUtc
			})
			// Order by the start date with newest at the top
			.OrderByDescending(x => x.Start)
			.ToListAsync();
	}

	public async Task<BasicEventDto> CreateEvent(NewBasicEventRequest newBasicEventRequest)
	{
		ValidateBasicEventRequest(newBasicEventRequest);

		var newEvent = new BasicEvent()
		{
			Description = newBasicEventRequest.Description!,
			Start = newBasicEventRequest.Start!.Value,
			End = newBasicEventRequest.End,
		};

		_context.BasicEvents.Add(newEvent);

		await _context.SaveChangesAsync();

		return new BasicEventDto()
		{
			Description = newEvent.Description,
			End = newEvent.End,
			Id = newEvent.Id,
			Start = newEvent.Start,
			// TODO
			//Tags =
			Version = newEvent.Version,
			CreatedUtc = newEvent.CreatedUtc,
			UpdatedUtc = newEvent.UpdatedUtc
		};
	}

	public async Task<BasicEventDto> UpdateEvent(UpdateBasicEventRequest updateBasicEventRequest)
	{
		ValidateBasicEventRequest(updateBasicEventRequest);

		var existingEvent = await _context.BasicEvents
			.FirstOrDefaultAsync(x => x.Id == updateBasicEventRequest.Id);

		if (existingEvent == null)
		{
			throw new Exception($"No event found with the id of {updateBasicEventRequest.Id}");
		}

		existingEvent.Description = updateBasicEventRequest.Description!;
		existingEvent.Start = updateBasicEventRequest.Start!.Value;
		existingEvent.End = updateBasicEventRequest.End;
		existingEvent.Id = existingEvent.Id;
		// TODO
		//existingEvent.Tags = updateBasicEventRequest.Tags,

		// If the event is updated elsewhere this will throw a concurrency error
		await _context.SaveChangesAsync();

		return new BasicEventDto()
		{
			Description = existingEvent.Description,
			End = existingEvent.End,
			Id = existingEvent.Id,
			Start = existingEvent.Start,
			//Tags =
			Version = existingEvent.Version,
			CreatedUtc = existingEvent.CreatedUtc,
			UpdatedUtc = existingEvent.UpdatedUtc
		};
	}

	private void ValidateBasicEventRequest(BasicEventRequest eventRequest)
	{
		if (string.IsNullOrWhiteSpace(eventRequest.Description))
		{
			throw new Exception("Description cannot be null");
		}

		if (eventRequest.Start == null)
		{
			throw new Exception("Start date cannot be null");
		}

		if (eventRequest.End != null && eventRequest.End <= eventRequest.Start)
		{
			throw new Exception("Start date cannot be exactly on or after end date");
		}
	}
}