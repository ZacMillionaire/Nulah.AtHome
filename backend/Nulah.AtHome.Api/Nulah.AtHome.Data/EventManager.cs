using Microsoft.Extensions.Logging;
using Nulah.AtHome.Data.DTO;

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
		await Task.Delay(0);

		return _context.BasicEvents.Select(x => new BasicEventDto()
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
			.ToList();
	}
}