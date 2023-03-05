using Microsoft.Extensions.Logging;

namespace Xkit.Logging.ConsoleLogger;

public class XkitConsoleLoggerConfiguration
{
	public XkitConsoleLoggerConfiguration()
	{
		
	}
	// public int EventId { get; set; }

	public Dictionary<LogLevel, ConsoleColor> LogLevels { get; set; } = new()
	{
		[LogLevel.Critical] = ConsoleColor.Red,
		[LogLevel.Error] = ConsoleColor.Red,
		[LogLevel.Warning] = ConsoleColor.Yellow,
		[LogLevel.Information] = ConsoleColor.Cyan,
		[LogLevel.Debug] = ConsoleColor.Gray,
		[LogLevel.Trace] = ConsoleColor.DarkGray,
	};

	public string DateTimeFormat = "MM'/'dd HH:mm:ss.fff"; // custom log friendly format without year, zero leading for sorting and search
	public bool DateTimeUtc = false; // false, because console supposed to be local data view. On servers and containers it will be UTC anyway.
}
