using Microsoft.Extensions.Logging;
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
        
        public static ConcurrentBag<DebugInfo> cd = new ConcurrentBag<DebugInfo>();
        Stopwatch sw = new Stopwatch();
        DebugInfo dt;
        string text;
        public StartLogging(string methodName, string className, int lineNumber)
        {
            
            dt.StartTime = DateTime.Now;
            dt.ClassName = className;
            dt.MethodName = methodName;
            dt.LineNumber = lineNumber;
            sw.Start();
            text = methodName + " from " + className + ":" + lineNumber;
            Log(LogLevel.Information, 0, " start method " + text, null, null);
            
        }

        

        public void Dispose()
        {
            Debug.WriteLine("test debug");
            
            sw.Stop();
            dt.duration = sw.Elapsed;
            cd.Add(dt);
            text = " end method " + text + " duration:" + dt.duration.TotalMilliseconds;           
            Log(LogLevel.Information, 0, text, null, null);
            
            
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
               Console.WriteLine($"{logLevel}  {state}");
                if (exception != null)
                {
                    var x = exception.Message +Environment.NewLine+ exception.StackTrace;
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

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}