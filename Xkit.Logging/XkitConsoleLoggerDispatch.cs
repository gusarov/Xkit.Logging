using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;

namespace Xkit.Logging.ConsoleLogger;

internal class XkitConsoleLoggerDispatch
{
	private XkitConsoleLoggerProvider _customConsoleLoggerProvider;
	private ConcurrentQueue<Entry> _entries = new ConcurrentQueue<Entry>();
	private AutoResetEvent _event = new AutoResetEvent(false);

	public XkitConsoleLoggerDispatch(XkitConsoleLoggerProvider customConsoleLoggerProvider)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;

		_customConsoleLoggerProvider = customConsoleLoggerProvider;
		new Thread(Worker)
		{
			IsBackground = true,
			Priority = ThreadPriority.BelowNormal,
		}.Start();
	}

	private void Worker(object? obj)
	{
		while (true)
		{
			if (_event.WaitOne(TimeSpan.FromSeconds(5)))
			{
				while (_entries.TryDequeue(out var entry))
				{
					LogForReal(entry);
				}
			}
		}
	}

	private void LogForReal(Entry entry)
	{
		var originalColor = Console.ForegroundColor;
		var color = GetColor(entry);
		Console.ForegroundColor = Dark(color);
		Console.Write($"[{entry.Time:MM'/'dd HH:mm:ss}] {Level(entry.LogLevel).ToUpperInvariant()}: {entry.StateType}: ");
		Console.ForegroundColor = color;
		Console.WriteLine(entry.Message.Replace("\n", "\n\t"));
		if (entry.MessageDarkExtra != null)
		{
			Console.ForegroundColor = Dark(color, true);
			Console.WriteLine(entry.MessageDarkExtra);
		}
		if (entry.Exception != null)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(entry.Exception);
		}
		Console.ForegroundColor = originalColor;
	}

	string Level(LogLevel level)
	{
		switch (level)
		{
			case LogLevel.Trace:
				return "trc";
			case LogLevel.Debug:
				return "dbg";
			case LogLevel.Information:
				return "inf";
			case LogLevel.Warning:
				return "wrn";
			case LogLevel.Error:
				return "err";
			case LogLevel.Critical:
				return "crt";
			case LogLevel.None:
				return "non";
			default:
				return "def";
		}
	}

	ConsoleColor GetColor(Entry entry)
	{
		if (_customConsoleLoggerProvider.GetConfig().LogLevels.TryGetValue(entry.LogLevel, out var color))
		{
			return color;
		}
		return ConsoleColor.Gray;
	}

	ConsoleColor Dark(ConsoleColor color, bool fallToGray = false)
	{
		switch (color)
		{
			case ConsoleColor.DarkBlue:
			case ConsoleColor.DarkGreen:
			case ConsoleColor.DarkCyan:
			case ConsoleColor.DarkRed:
			case ConsoleColor.DarkMagenta:
			case ConsoleColor.DarkYellow:
			case ConsoleColor.DarkGray:
			case ConsoleColor.Black:
				return fallToGray ? ConsoleColor.DarkGray : color;
			case ConsoleColor.Gray:
				return ConsoleColor.DarkGray;
			case ConsoleColor.Blue:
				return ConsoleColor.DarkBlue;
			case ConsoleColor.Green:
				return ConsoleColor.DarkGreen;
			case ConsoleColor.Cyan:
				return ConsoleColor.DarkCyan;
			case ConsoleColor.Red:
				return ConsoleColor.DarkRed;
			case ConsoleColor.Magenta:
				return ConsoleColor.DarkMagenta;
			case ConsoleColor.Yellow:
				return ConsoleColor.DarkYellow;
			case ConsoleColor.White:
				return ConsoleColor.Gray;
			default:
				return ConsoleColor.Gray;
		}
	}

	public void Log(Entry entry)
	{
		_entries.Enqueue(entry);
		_event.Set();
	}
}
