using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Logging
{

    public class StartLogging : Microsoft.Extensions.Logging.ILogger, IDisposable
    {
        
        static ILoggerFactory fact;
        static StartLogging()
        {
            try
            {
                //var configuration = new ConfigurationBuilder()
                //.AddJsonFile("serilogsettings.json")
                //.Build();
                //TODO: use from .NET 2.0
                //var logger = new LoggerConfiguration()
                //    .ReadFrom.Configuration(configuration)
                //    .CreateLogger();
                string template = "[{Timestamp:HH:mm:ss} Thread {ThreadId} {Scope} {Level:u3}] {Message:l}{NewLine}{Exception}";
                Serilog.Log.Logger = new LoggerConfiguration()
                    .Enrich.WithThreadId()
                    .WriteTo.LiterateConsole(outputTemplate: template)
                    .WriteTo.Async(a => a.RollingFile("log-{Date}.txt", shared: true, retainedFileCountLimit: null, outputTemplate: template))
                    .CreateLogger();



                var sc = new ServiceCollection();
                sc.AddLogging();
                var loggerFactory = sc.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
                fact = loggerFactory.AddSerilog(Serilog.Log.Logger, false);
                //TODO: dispose the logger when applications shuts down
            }
            catch(Exception ex)
            {
                Console.Write("exception serilog" + ex.Message);
            }

        }
        public static ConcurrentBag<DebugInfo> cd = new ConcurrentBag<DebugInfo>();
        Stopwatch sw = new Stopwatch();
        DebugInfo dt;
        string text;
        Microsoft.Extensions.Logging.ILogger logger;
        public StartLogging(string methodName, string className, int lineNumber)
        {
            dt.StartTime = DateTime.Now;
            dt.ClassName = className;
            dt.MethodName = methodName;
            dt.LineNumber = lineNumber;
            sw.Start();
            text = methodName + " from " + className + ":" + lineNumber;
            if (fact != null)
            {
                logger = fact.CreateLogger(className);
                logger.LogInformation(" start method " + text);
            }
            else
            {
                Log(LogLevel.Information, 0, " start method " + text, null, null);
            }
        }
        
        public void Dispose()
        {
            Debug.WriteLine("test debug");
            
            sw.Stop();
            dt.duration = sw.Elapsed;
            cd.Add(dt);
            text = " end method " + text + " duration:" + dt.duration.TotalMilliseconds;
            if (logger != null)
            { 
                logger.LogInformation(text);
                Serilog.Log.CloseAndFlush();
            }
            else
            {
                Log(LogLevel.Information, 0, text, null, null);
            }
            
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logger != null)
            {
                logger.Log(logLevel, eventId, state, exception, formatter);
            }
            else
            {
                //TODO: use serilog
                Console.WriteLine($"{logLevel}  {state}");
                if (exception != null)
                {
                    var x = exception.Message;
                    if (formatter != null)
                    {
                        x = formatter(state, exception);
                    }
                    var cc = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(x);
                    Console.ForegroundColor = cc;

                }
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return logger.BeginScope(state);
        }
    }
}