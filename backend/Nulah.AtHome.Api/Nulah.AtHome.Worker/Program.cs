using Nulah.AtHome.Worker;
using RabbitMQ.Client;

internal class Program
{
	public static void Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.Services.AddHostedService<Worker>();
		builder.Services.AddSingleton(new ConnectionFactory { HostName = "localhost" }.CreateConnection());

		var host = builder.Build();
		host.Run();
	}
}