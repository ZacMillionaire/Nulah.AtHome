using Microsoft.AspNetCore.Mvc;
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
	public async Task<ActionResult<BasicEventDto>> Create([FromBody] NewBasicEventRequest newBasicEventRequest)
	{
		try
		{
			var createdEvent = await _manager.CreateEvent(newBasicEventRequest);
			return createdEvent;
		}
		catch(Exception ex)
		{
			return new BadRequestObjectResult(ex.Message);
		}
	}
}