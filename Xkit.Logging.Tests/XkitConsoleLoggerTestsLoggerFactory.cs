using Microsoft.Extensions.Logging;
using Xkit.Logging.ConsoleLogger;

namespace Xkit.Logging.Tests;

// [DoNotParallelize]
[TestClass]
public class XkitConsoleLoggerTestsLoggerFactory
{
	ILogger _logger;

	// [TestInitialize]
	public void Setup()
	{
		return;
		// Setup Console
		var ms = new MemoryStream();
		var sw = new StreamWriter(ms);
		Console.SetOut(sw);

		// Setup Host
		var loggerFactory = LoggerFactory.Create(b =>
		{
			b.AddXkitConsoleLogger();
		});
		_logger = loggerFactory.CreateLogger<XkitConsoleLoggerTestsLoggerFactory>();
	}

	[TestMethod]
	public void Should_10_log_string_as_is()
	{
		return;
		_logger.LogWarning("The Test Warning");
		Assert.Inconclusive();
	}


	[TestMethod]
	public void Should_15_log_only_when_level_is_enabled()
	{
		return;
		Assert.Inconclusive();
	}
}