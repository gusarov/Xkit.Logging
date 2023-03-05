namespace Xkit.Logging.ConsoleLogger;

static internal class Extensions
{
	public static DateTime ToLocalIfConfigured(this DateTime dateTime, XkitConsoleLoggerConfiguration config)
	{
		return config.DateTimeUtc
			? dateTime
			: dateTime.ToLocalTime();
	}
}
