using Microsoft.Extensions.Logging;
using Xkit.Logging.ConsoleLogger;

namespace Xkit.Logging.Tests;

[DoNotParallelize]
public class XkitConsoleLoggerTestsHost
{
	IHostBuilder _hostBuilder;
	IHost _host;

	[TestInitialize]
	public void Setup()
	{
		// Setup Console
		var ms = new MemoryStream();
		var sw = new StreamWriter(ms);
		Console.SetOut(sw);

		// Setup Host
		_hostBuilder = Host.CreateDefaultBuilder()
			.ConfigureServices((hbCtx, sc) =>
			{

			})
			.ConfigureLogging((hbCtx, lb) =>
			{
				// XkitConsoleLoggerExtensions
				lb.AddXkitConsoleLogger();
			});

		// var sc = new ServiceCollection();
		// sc.AddSingleton<>
		// _sp = sc.BuildServiceProvider();

		_host = _hostBuilder.Start();
	}

	[TestCleanup]
	public void TearDown()
	{
		_host.Dispose();
		_host.WaitForShutdown();
	}

	[TestMethod]
	public void Should_10_log_string_as_is()
	{
		var logger = _host.Services.GetRequiredService<ILogger>();
		Assert.Inconclusive();
	}


	[TestMethod]
	public void Should_15_log_only_when_level_is_enabled()
	{
		Assert.Inconclusive();
	}
}