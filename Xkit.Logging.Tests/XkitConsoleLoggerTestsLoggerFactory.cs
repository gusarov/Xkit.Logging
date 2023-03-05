using System;
using Microsoft.Extensions.Logging;
using Xkit.Logging.ConsoleLogger;

namespace Xkit.Logging.Tests;

// [DoNotParallelize]
[TestClass]
public class XkitConsoleLoggerTestsLoggerFactory
{
	XkitConsoleLoggerConfiguration _config;
	ILogger _logger;
	MemoryStream _stream;
	StreamWriter _streamWriter;
	StreamReader _streamReaderStorage;
	StreamReader _streamReader
	{
		get
		{
			if (_streamReaderStorage == null)
			{
				Thread.Sleep(300); // separate thread for logging. Could be improved by exposing event waithandle from XkitConsoleLoggerDispatch
				_streamWriter.Flush();
				_stream.Position = 0;
				_streamReaderStorage = new StreamReader(_stream, leaveOpen: true);
			}
			return _streamReaderStorage;
		}
	}

	[TestInitialize]
	public void Setup()
	{
		// Setup Console
		_stream = new MemoryStream();
		_streamWriter = new StreamWriter(_stream, leaveOpen: true);
		_streamReaderStorage = null;
		Console.SetOut(_streamWriter);

		// Setup Host
		var loggerFactory = LoggerFactory.Create(b =>
		{
			b.AddXkitConsoleLogger(cfg =>
			{
				_config = cfg;
			});
		});
		_logger = loggerFactory.CreateLogger<XkitConsoleLoggerTestsLoggerFactory>();
	}

	[TestMethod]
	public void Should_10_log_string_as_is()
	{
		_logger.LogWarning("The Test Warning");
		var line = _streamReader.ReadLine();
		Assert.IsTrue(line?.Contains("The Test Warning") ?? false, line);
	}

	static DateTime _dateTimeUtcNow = new DateTime(2000, 1, 2, 3, 4, 5);

	[TestMethod]
	public void Should_11_log_string_in_specific_format()
	{
		_config.DateTimeUtc = true;
		Injector.Replace(() => DateTime.UtcNow, () => _dateTimeUtcNow);

		_logger.LogInformation("The Test Time");
		var line = _streamReader.ReadLine();
		Assert.AreEqual("[01/02 03:04:05.000] inf: The Test Time", line);
	}

	[TestMethod]
	public void Should_15_log_only_when_level_is_enabled()
	{
		_logger.LogDebug("The Test Debug");
		var line = _streamReader.ReadLine();
		Assert.IsFalse(line?.Contains("The Test Debug") ?? false, line);
	}
}