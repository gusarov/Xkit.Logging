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

public static class XkitConsoleLoggerExtensions
{
	public static ILoggingBuilder AddXkitConsoleLogger(this ILoggingBuilder builder)
	{
		builder.AddConfiguration();
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, XkitConsoleLoggerProvider>());
		LoggerProviderOptions.RegisterProviderOptions<XkitConsoleLoggerConfiguration, XkitConsoleLoggerProvider>(builder.Services);
		return builder;
	}

	public static ILoggingBuilder AddXkitConsoleLogger(this ILoggingBuilder builder, Action<XkitConsoleLoggerConfiguration> configure)
	{
		builder.AddXkitConsoleLogger();
		builder.Services.Configure(configure);
		return builder;
	}
}
