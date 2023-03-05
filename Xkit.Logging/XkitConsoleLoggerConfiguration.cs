using Microsoft.Extensions.Logging;

namespace Xkit.Logging.ConsoleLogger;

public class XkitConsoleLoggerConfiguration
{
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
}
