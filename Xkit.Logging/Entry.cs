using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Xkit.Logging.ConsoleLogger;

public abstract class Entry
{
	public DateTime Time;
	public string? LoggerName;
	public LogLevel LogLevel;
	public EventId EventId;
	public Exception? Exception;
	public Type? StateType { get; set;  }

	public abstract string Message { get; }
	public virtual string? MessageDarkExtra => null;
}

/*
public class Entry<TState> : Entry
{
	public Func<TState, Exception, string> Formatter;
	public TState State;
	public override string Message => Formatter(State, Exception);
}
*/

public class EntryString : Entry
{
	/// <summary>
	/// Pre-formatted message here (if excpetion or status exists, it is embedded and rendered into string)
	/// </summary>
	public string? MessageFormatted;
	public string? MessageDarkExtraFormatted;

	public override string Message => MessageFormatted ?? throw new InvalidOperationException("Message is not provided");
	public override string? MessageDarkExtra => MessageDarkExtraFormatted;
}
