using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Xkit.Logging.ConsoleLogger;


internal class XkitConsoleLogger : ILogger
{
	private readonly string _name;
	private readonly Func<XkitConsoleLoggerConfiguration> _getConfig;
	private readonly XkitConsoleLoggerDispatch _dispatch;

	internal XkitConsoleLogger(
		string name,
		Func<XkitConsoleLoggerConfiguration> getCurrentConfig,
		XkitConsoleLoggerDispatch dispatch)
	{
		_name = name;
		_getConfig = getCurrentConfig;
		_dispatch = dispatch;
	}

	private class Scope : IDisposable
	{
		private readonly Action _act;

		public Scope([NotNull] Action act)
		{
			_act = act ?? throw new ArgumentNullException(nameof(act));
		}
		public void Dispose()
		{
			_act();
		}
	}

	IDisposable? ILogger.BeginScope<TState>(TState state)
	{
		return default;
		/*
		var g = Guid.NewGuid();
		_dispatch.Log(new EntryString
		{
			Time = DateTime.UtcNow,
			LoggerName = _name,
			LogLevel = LogLevel.Information,
			// EventId = eventId,
			// Exception = exception,
			StateType = state.GetType(),
			MessageFormatted = "Begin Scope " + g + state,
			// MessageDarkExtraPreform = customMessageDarkExtra,
		});
		return new Scope(() =>
		{
			_dispatch.Log(new EntryString
			{
				Time = DateTime.UtcNow,
				LoggerName = _name,
				LogLevel = LogLevel.Information,
				// EventId = eventId,
				// Exception = exception,
				StateType = state.GetType(),
				MessageFormatted = "End Scope " + g + state,
				// MessageDarkExtraPreform = customMessageDarkExtra,
			});

		});
		*/
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return _getConfig().LogLevels.ContainsKey(logLevel);
	}

	void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}
		/*
		_dispatch.Log(new Entry<TState>
		{
			Time = DateTime.UtcNow,
			LoggerName = _name,
			LogLevel = logLevel,
			EventId = eventId,
			State = state,
			Exception = exception,
			Formatter = formatter,
		});
		*/

		string? customMessage = null;
		string? customMessageDarkExtra = null; // extra hint message with darkened console color
		if (state != null)
		{
			if (state.GetType().Name == "HttpRequestLog")
			{
				if (state is IEnumerable<KeyValuePair<string, object?>> kvp)
				{
					var dic = kvp.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
					customMessage = $"{dic.GetValueOrDefault("Method")} {dic.GetValueOrDefault("Scheme")}://{dic.GetValueOrDefault("Host") ?? "+"}{dic.GetValueOrDefault("PathBase")}{dic.GetValueOrDefault("Path")} {dic.GetValueOrDefault("Protocol")}";
					/*
					foreach (var item in kvp)
					{
						switch (item.Key.ToLowerInvariant())
						{
							case "method":
							case "scheme":
							case "host":
							case "path":
							case "pathbase":
							case "protocol":
								break;
							default:
								customMessageDarkExtra += $"<{item.Key}> = <{item.Value}>\r\n";
								break;
						}
					}
					*/
				}
			}
			if (state.GetType().Name == "HttpResponseLog")
			{
				if (state is IEnumerable<KeyValuePair<string, object?>> kvp)
				{
					// customMessage = $"StatusCode: {state.}";
					var dic = kvp.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
					customMessage = $"Response: {dic.GetValueOrDefault("StatusCode")}";
					/*
					foreach (var item in kvp)
					{
						switch (item.Key.ToLowerInvariant())
						{
							case "statuscode":
								break;
							default:
								if (item.Value is string s && s != "[Redacted]")
								{
									customMessageDarkExtra += $"\t{item.Key}: {s}\r\n";
								}
								break;
						}
					}
					*/
				}
			}
		}

		_dispatch.Log(new EntryString
		{
			Time = DateTime.UtcNow,
			LoggerName = _name,
			LogLevel = logLevel,
			EventId = eventId,
			Exception = exception,
			StateType = state?.GetType(),
			MessageFormatted = customMessage ?? formatter(state, exception) /*+ ExceptionFormatter(exception)*/,
			MessageDarkExtraFormatted = customMessageDarkExtra,
		});
		// Console.WriteLine("CUST: " + );
	}

	/*
	private string ExceptionFormatter(Exception exception)
	{
		if (exception == null)
		{
			return null;
		}
		return Environment.NewLine + exception.ToString();
	}
	*/
}
