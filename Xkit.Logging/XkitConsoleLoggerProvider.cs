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

internal sealed class XkitConsoleLoggerProvider : ILoggerProvider
{
	private readonly IDisposable? _onChangeSubscription;
	private XkitConsoleLoggerConfiguration _config;
	private readonly ConcurrentDictionary<string, XkitConsoleLogger> _loggers = new();
	private XkitConsoleLoggerDispatch _dispatch;

	public XkitConsoleLoggerProvider(IOptionsMonitor<XkitConsoleLoggerConfiguration> config)
	{
		_config = config.CurrentValue;
		_onChangeSubscription = config.OnChange(updatedConfig => _config = updatedConfig);
		_dispatch = new XkitConsoleLoggerDispatch(this);
	}

	public ILogger CreateLogger(string categoryName) =>
		_loggers.GetOrAdd(categoryName, name => new XkitConsoleLogger(name, GetConfig, _dispatch));

	public XkitConsoleLoggerConfiguration GetConfig() => _config;

	public void Dispose()
	{
		_loggers.Clear();
		_onChangeSubscription?.Dispose();
	}
}
