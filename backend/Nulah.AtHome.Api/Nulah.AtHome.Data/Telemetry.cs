using System.Diagnostics;
using OpenTelemetry.Trace;

namespace Nulah.AtHome.Data;

public static class Telemetry
{
	private const string ServiceName = "Nulah.AtHome.Data";

	public static ActivitySource ActivitySource => new(ServiceName, "2024.0.0");

	/// <summary>
	/// Adds the Nulah.AtHome.Data instrumentation source
	/// </summary>
	/// <param name="builder"></param>
	/// <returns></returns>
	public static TracerProviderBuilder AddNulahDataInstrumentation(this TracerProviderBuilder builder)
	{
		if (builder is IDeferredTracerProviderBuilder deferredTracerProviderBuilder)
		{
			deferredTracerProviderBuilder.Configure((sp, b) => { b.AddNulahAtHomeDataInstrumentationSources(sp); });
		}

		return builder;
	}

	/// <summary>
	/// Honestly doesn't need to be here, but considering I adapted this from code that Asp uses to add instrumentation
	/// I stayed lazy and just left it here
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="serviceProvider"></param>
	private static void AddNulahAtHomeDataInstrumentationSources(this TracerProviderBuilder builder,
		IServiceProvider serviceProvider)
	{
		// All this does is add the service name for the activity to the trace provider so it knows to include
		// any activities and tracing we 
		builder.AddSource(ServiceName);
	}
}