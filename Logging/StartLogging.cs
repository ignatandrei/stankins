using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logging
{

    public class StartLogging : ILogger, IDisposable
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
            text = methodName + " from " + className;
            Console.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz") + " start " + text + lineNumber);
        }
        
        public void Dispose()
        {
            Debug.WriteLine("test debug");
            
            sw.Stop();
            dt.duration = sw.Elapsed;
            cd.Add(dt);
            Console.WriteLine((DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz") + " end " + text + "duration:" + dt.duration.TotalMilliseconds));
            
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {            
            //TODO: use serilog
            Console.WriteLine($"{logLevel}  {state}");
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