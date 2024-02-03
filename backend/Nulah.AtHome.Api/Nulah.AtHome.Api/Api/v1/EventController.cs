using Microsoft.AspNetCore.Mvc;
using Nulah.AtHome.Api.Models;
using Nulah.AtHome.Data;
using Nulah.AtHome.Data.DTO;

namespace Nulah.AtHome.Api.Api.v1;

[ApiController]
[Route("Api/v1/Events")]
public class EventController : ControllerBase
{
	private readonly EventManager _manager;
	private readonly ILogger _logger;

	public EventController(EventManager eventManager, ILogger<EventController> logger)
	{
		_manager = eventManager;
		_logger = logger;
	}

	[HttpGet]
	[Route("Get")]
	public async Task<ActionResult<List<BasicEventDto>>> Get()
	{
		var events = await _manager.GetEvents();

		return events;
	}

	[HttpPost]
	[Route("Create")]
	public async Task<IActionResult> Create([FromBody] NewBasicEventRequest newBasicEventRequest)
	{
		var validationErrors = ValidateNewEvent(newBasicEventRequest);

		if (validationErrors.Count > 0)
		{
			return new BadRequestObjectResult(validationErrors);
		}

		try
		{
			var createdEvent = await _manager.CreateEvent(newBasicEventRequest);
			return Ok(createdEvent);
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(ex.Message);
		}
	}

	private List<ErrorResponse> ValidateNewEvent(NewBasicEventRequest newBasicEventRequest)
	{
		var errors = new List<ErrorResponse>();

		if (string.IsNullOrWhiteSpace(newBasicEventRequest.Description))
		{
			errors.Add(new ErrorResponse()
			{
				Name = nameof(NewBasicEventRequest.Description),
				Description = "Description cannot be null"
			});
		}

		if (newBasicEventRequest.Start == null)
		{
			errors.Add(new ErrorResponse()
			{
				Name = nameof(NewBasicEventRequest.Start),
				Description = "Start date cannot be null"
			});
		}

		if (newBasicEventRequest.End != null && newBasicEventRequest.Start <= newBasicEventRequest.End)
		{
			errors.AddRange(new[]
			{
				new ErrorResponse()
				{
					Name = nameof(NewBasicEventRequest.Start),
					Description = "Start date cannot be exactly on or after end date"
				},
				new ErrorResponse()
				{
					Name = nameof(NewBasicEventRequest.End),
					Description = "End date cannot be exactly on or before start date"
				}
			});
		}

		return errors;
	}
}