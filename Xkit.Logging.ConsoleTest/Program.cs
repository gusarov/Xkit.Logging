using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xkit.Logging.ConsoleLogger;

Host.CreateDefaultBuilder()
	.ConfigureServices(sc =>
	{
		sc.AddLogging(lb =>
		{
			lb.ClearProviders();
			lb.AddXkitConsoleLogger();
		});
		sc.AddSingleton<DemoServiceClass>();
	})
	.Start()
	.Services.GetRequiredService<DemoServiceClass>()
	.RunDemo();

Thread.Sleep(1000);

class DemoServiceClass
{
	private readonly ILogger _logger;

	public DemoServiceClass(ILogger<DemoServiceClass> logger)
	{
		_logger = logger ?? throw new ArgumentNullException();
	}

	public void RunDemo()
	{
		_logger.LogInformation("1. BASIC LEVELS:");
		_logger.Log(LogLevel.Critical, "The Critical");
		_logger.Log(LogLevel.Error, "The Error");
		_logger.Log(LogLevel.Warning, "The Warning");
		_logger.Log(LogLevel.Information, "The Information");
		_logger.Log(LogLevel.Debug, "The Debug");
		_logger.Log(LogLevel.Trace, "The Trace");

		var nl = Environment.NewLine;
		_logger.LogInformation($"2. MULTILINE: 1 This is first line{nl}2 Second end with {nl}3 Third end with {nl}4 Fourth line");

		// create real exception with real stack
		Exception exx;
		try
		{
			throw null;
		}
		catch (Exception ex)
		{
			exx = ex;
		}

		_logger.LogInformation("3. CUSTOM STATE FORMATTER:");
		_logger.Log(LogLevel.Information, new EventId(1, "DemoEventId"), new DemoLogState(), exx, (s, e) => $"F1: {s.Field1} F2: {s.Field2} E: {e?.Message}");

		_logger.LogInformation("4. EXCEPTION:");
		_logger.LogError(new EventId(2, "DemoErrorEventId"), exx, "DemoFailure");
	}

	class DemoLogState
	{
		public int Field1 { get; set; } = 777;
		public string Field2 { get; set; } = "Some Data String";
	}
}

