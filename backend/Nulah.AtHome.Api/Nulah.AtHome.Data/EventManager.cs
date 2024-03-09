using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nulah.AtHome.Data.Converters;
using Nulah.AtHome.Data.DTO;
using Nulah.AtHome.Data.DTO.Events;
using Nulah.AtHome.Data.Models;
using Nulah.AtHome.Data.Models.Events;
using OpenTelemetry.Trace;

namespace Nulah.AtHome.Data;

public class EventListCriteria
{
	public bool? HasEventDate { get; set; }
	public DateTimeOffset? BeforeEndDate { get; set; }
}

public class EventManager
{
	private readonly AppDbContext _context;
	private readonly ILogger _logger;

	public EventManager(AppDbContext context, ILogger<EventManager> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task<List<BasicEventDto>> GetEvents(EventListCriteria? criteria = null)
	{
		return await _context.BasicEvents
			.Where(Build(criteria))
			.Select(x => new BasicEventDto()
			{
				Description = x.Description,
				End = x.End,
				Id = x.Id,
				Start = x.Start,
				Tags = x.Tags
					.Select(y => new TagDto()
					{
						Id = y.Id,
						Name = y.Name,
						Version = y.Version
					})
					.OrderBy(y => y.Id)
					.ToList(),
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
		using var updateEvent = Telemetry.ActivitySource.StartActivity(ActivityKind.Internal);
		PopulateActivityTags(updateEvent, newBasicEventRequest);

		ValidateBasicEventRequest(newBasicEventRequest);

		var newEvent = new BasicEvent()
		{
			Description = newBasicEventRequest.Description!,
			Start = newBasicEventRequest.Start!.Value,
			End = newBasicEventRequest.End,
			Tags = FindTags(SanitiseTags(newBasicEventRequest.Tags))
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
			Tags = newEvent.Tags
				.Select(x => new TagDto()
				{
					Id = x.Id,
					Name = x.Name,
					Version = x.Version
				})
				.OrderBy(y => y.Id)
				.ToList(),
			Version = newEvent.Version,
			CreatedUtc = newEvent.CreatedUtc,
			UpdatedUtc = newEvent.UpdatedUtc
		};
	}

	public async Task<BasicEventDto> UpdateEvent(UpdateBasicEventRequest updateBasicEventRequest)
	{
		using var updateEvent = Telemetry.ActivitySource.StartActivity(ActivityKind.Internal);
		updateEvent?.SetTag("event.id", updateBasicEventRequest.Id);
		updateEvent?.SetTag("event.version", updateBasicEventRequest.Version);
		PopulateActivityTags(updateEvent, updateBasicEventRequest);

		try
		{
			return await UpdateEventAsync(updateBasicEventRequest);
		}
		catch (Exception ex)
		{
			updateEvent?.SetStatus(ActivityStatusCode.Error, ex.Message);
			updateEvent?.RecordException(ex);
			throw;
		}
	}

	public async Task<BasicEventStatsDto> GetStats()
	{
		var events = await _context.BasicEvents
			.Select(x => new
			{
				HasEndDate = x.End == null,
				HasTags = x.Tags.Count > 0
			})
			.ToListAsync();

		var stats = new BasicEventStatsDto()
		{
			Total = events.Count,
			WithTags = events.Count(x => x.HasTags),
			WithoutTags = events.Count(x => !x.HasTags),
			WithEndDate = events.Count(x => x.HasEndDate),
			WithoutEndDate = events.Count(x => !x.HasEndDate),
		};

		return stats;
	}


	private Expression<Func<BasicEvent, bool>> Build(EventListCriteria? criteria)
	{
		if (criteria == null)
		{
			return x => true;
		}

		Expression<Func<BasicEvent, bool>>? baseFunc = null;

		if (criteria.HasEventDate.HasValue)
		{
			if (criteria.HasEventDate.Value)
			{
				baseFunc = baseFunc.And(x => x.End != null);
			}
			else
			{
				baseFunc = baseFunc.And(x => x.End == null);
			}
		}

		// Return an empty expression
		baseFunc ??= x => true;

		if (baseFunc.CanReduce)
		{
			baseFunc.Reduce();
		}

		return baseFunc;
	}

	private async Task<BasicEventDto> UpdateEventAsync(UpdateBasicEventRequest updateBasicEventRequest)
	{
		using var updateActivity = Telemetry.ActivitySource.StartActivity(ActivityKind.Internal);

		ValidateBasicEventRequest(updateBasicEventRequest);

		updateActivity?.AddEvent(new ActivityEvent("Find event by Id and Version", DateTimeOffset.Now));

		var existingEvent = await _context.BasicEvents
			.Include(basicEvent => basicEvent.Tags)
			.FirstOrDefaultAsync(x =>
				x.Id == updateBasicEventRequest.Id
				// this is ugly but I want to make sure that we're definitely getting the version.
				// If we just load by raw Id then we'll completely sidestep version validation
				&& x.Version == updateBasicEventRequest.Version
			);

		if (existingEvent == null)
		{
			throw new Exception($"No event found with the id of {updateBasicEventRequest.Id} and version {updateBasicEventRequest.Version}");
		}

		existingEvent.Description = updateBasicEventRequest.Description!;
		existingEvent.Start = updateBasicEventRequest.Start!.Value;
		existingEvent.End = updateBasicEventRequest.End;

		// Determine new/existing tags as usual
		var generateTags = FindTags(SanitiseTags(updateBasicEventRequest.Tags));

		// Get a list of tags that have been removed
		// This always feels really gross whenever I do concepts like this
		var removedTags = updateBasicEventRequest.Tags != null && updateBasicEventRequest.Tags.Count != 0
			? existingEvent.Tags
				.Where(x => updateBasicEventRequest.Tags.Any(y => y != x.Name))
				.ToList()
			: [];

		// Assign the union of current tags to the existing/new tags list, while removing
		// ones as appropriate
		existingEvent.Tags = existingEvent.Tags
			.Except(removedTags)
			.Union(generateTags)
			.DistinctBy(x => x.Id)
			.ToList();

		// If the event is updated elsewhere this will throw a concurrency error, but if we've made it this far we can
		// be certain we've loaded the most recent version into tracking
		await _context.SaveChangesAsync();

		return new BasicEventDto()
		{
			Description = existingEvent.Description,
			End = existingEvent.End,
			Id = existingEvent.Id,
			Start = existingEvent.Start,
			Tags = existingEvent.Tags
				.Select(x => new TagDto()
				{
					Id = x.Id,
					Name = x.Name,
					Version = x.Version
				})
				.OrderBy(y => y.Id)
				.ToList(),
			Version = existingEvent.Version,
			CreatedUtc = existingEvent.CreatedUtc,
			UpdatedUtc = existingEvent.UpdatedUtc
		};
	}

	/// <summary>
	/// Given a list of strings, returns any existing tags that match by value from the database.
	/// <para>
	///	If a tag doesn't exist, a new tag will be returned per string to be generated when the query is executed
	/// </para>
	/// </summary>
	/// <param name="tagList"></param>
	/// <returns></returns>
	private List<Tag> FindTags(List<string> tagList)
	{
		using var updateActivity = Telemetry.ActivitySource.StartActivity(ActivityKind.Internal);
		updateActivity?.SetTag("tags", string.Join(",", tagList));

		if (tagList.Count == 0)
		{
			updateActivity?.AddEvent(new ActivityEvent("No tags given", DateTimeOffset.Now));
			return [];
		}

		var existingTags = _context.Tags
			.Where(x => tagList.Contains(x.Name))
			.ToList();

		var newTags = tagList.Where(x => existingTags.All(y => y.Name != x))
			.Select(x => new Tag()
			{
				Name = x
			});

		existingTags.AddRange(newTags);

		return existingTags;
	}

	/// <summary>
	/// Returns a new list of tags with all leading and trailing whitespace removed, and any
	/// whitespace only entries removed
	/// </summary>
	/// <param name="tags"></param>
	/// <returns></returns>
	private List<string> SanitiseTags(List<string>? tags = null)
	{
		return tags == null || tags.Count == 0
			? []
			: tags.Select(x => x.Trim())
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList();
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

	private void PopulateActivityTags(Activity? activity, BasicEventRequest eventRequest)
	{
		activity?.SetTag("event.start", eventRequest.Start);
		activity?.SetTag("event.end", eventRequest.End);
		activity?.SetTag("event.description", eventRequest.Description);
		activity?.SetTag("event.tags",
			eventRequest.Tags is { Count: > 0 }
				? string.Join(",", eventRequest.Tags)
				: "[no tags]"
		);
	}
}