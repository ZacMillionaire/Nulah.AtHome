using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nulah.AtHome.Worker.Workers.Youtube;

public class YoutubeWorker
{
	private readonly ActivitySource _activitySource;
	private readonly IConnection _rabbitMqConnection;

	public YoutubeWorker(ActivitySource activitySource, IConnection rabbitMqConnection)
	{
		_activitySource = activitySource;
		_rabbitMqConnection = rabbitMqConnection;
	}

	public void StartWorker()
	{
		using var activity = _activitySource.StartActivity();

		var channel = _rabbitMqConnection.CreateModel();
		// auto generate a queue name for the binding
		var queueName = channel.QueueDeclare().QueueName;
		var consumer = new EventingBasicConsumer(channel);

		// binds to the message queue
		channel.QueueBind(queue: queueName,
			exchange: "topic_logs", // this should be from config for the queue name
			routingKey: "message.#"); // listen for all messages shoved into the queue name

		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			//var asdf = MessagePackSerializer.Deserialize<DiscordBotCore.ReactionDetails>(body);
			var routingKey = ea.RoutingKey;
			//Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
		};

		// auto ack any messages so they're consumed
		channel.BasicConsume(queue: queueName,
			autoAck: true,
			consumer: consumer);
	}
}