using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logging
{

    public class StartLogging :  IDisposable
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
            Console.WriteLine("start " + text + lineNumber);
        }
        public void LogDebug(string text)
        {

        }
        public void Dispose()
        {
            Debug.WriteLine("test debug");
            
            sw.Stop();
            dt.duration = sw.Elapsed;
            cd.Add(dt);
            Console.WriteLine("end " + text + "duration:" + dt.duration.TotalMilliseconds);
            
        }
    }
}