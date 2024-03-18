using System.Diagnostics;
using System.Text.Json;

namespace Nulah.AtHome.Worker;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;

	public Worker(ILogger<Worker> logger)
	{
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await RunYtdlp();

		while (!stoppingToken.IsCancellationRequested)
		{
			if (_logger.IsEnabled(LogLevel.Information))
			{
				_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
			}

			await Task.Delay(1000, stoppingToken);
		}
	}

	private async Task RunYtdlp()
	{
		Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
		var processStartInfo = new ProcessStartInfo()
		{
			FileName = "deps/yt-dlp.exe",
			Arguments = "--skip-download -J -I0 http://www.youtube.com/@Liquicity",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true
		};
		using var proc = new Process() { StartInfo = processStartInfo };
		proc.OutputDataReceived += (sender, args) =>
		{
			ParseChannelOutput(args.Data);
		};
		proc.ErrorDataReceived += (sender, args) =>
		{
			_logger.LogError("{stdErr}", args.Data);
		};
		proc.Start();
		proc.BeginOutputReadLine();
		var errors = await proc.StandardError.ReadToEndAsync();
		_logger.LogError(errors);
		await proc.WaitForExitAsync();
	}

	private void ParseChannelOutput(string? output)
	{
		if (string.IsNullOrWhiteSpace(output))
		{
			return;
		}

		_logger.LogInformation("{stdOut}", output);

		var a = JsonSerializer.Deserialize<ChannelDescription>(output);
	}
}

public class ChannelDescription
{
	public string id { get; set; }
	public string channel { get; set; }
	public string channel_id { get; set; }
	public string title { get; set; }
	public object availability { get; set; }
	public int channel_follower_count { get; set; }
	public string description { get; set; }
	public string[] tags { get; set; }
	public Thumbnails[] thumbnails { get; set; }
}

public class Thumbnails
{
	public string url { get; set; }
	public int height { get; set; }
	public int width { get; set; }
	public int preference { get; set; }
	public string id { get; set; }
	public string resolution { get; set; }
}