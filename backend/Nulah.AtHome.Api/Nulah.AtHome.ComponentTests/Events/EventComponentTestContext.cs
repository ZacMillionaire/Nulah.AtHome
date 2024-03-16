using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Nulah.AtHome.Api.Services;
using Nulah.AtHome.ComponentTests.Services;

namespace Nulah.AtHome.ComponentTests.Events;

public class EventComponentTestContext : TestContext
{
	internal readonly FakeTimeProvider TestTimeProvider = new();
	internal readonly TestEventService EventService;

	public EventComponentTestContext()
	{
		EventService = new(TestTimeProvider);
		Services.AddSingleton<IEventService>(EventService);
	}
}