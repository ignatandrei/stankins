using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StankinsTests
{
    [TestClass]
    public class FinalizeTest
    {

        [TestMethod]
        public void TestFake()
        {

            Console.WriteLine("Faketest");
            
        }
        [AssemblyCleanup]
        public static void FinishAllTest()
        {
            Console.WriteLine("finished test!");
            var data = Logging.StartLogging.cd.Select(it => new { Key=it.Key(), it.duration }).ToArray();
            var g = data.GroupBy(it => it.Key).ToArray();
            var meanTime= g.Select(it => new { it.Key, MilliSeconds = it.Sum(t => t.duration.TotalMilliseconds)/it.Count() }).OrderByDescending(s=>s.MilliSeconds).Take(10).ToArray();
            Console.WriteLine("!mean time");
            foreach(var item in meanTime)
            {
                Console.WriteLine($"!!!!!Method {item.Key} mean duration seconds {item.MilliSeconds/1000}");
            }
            Console.WriteLine("!max time");
            var totalTime= g.Select(it => new { it.Key, MilliSeconds = it.Sum(t => t.duration.TotalMilliseconds) ,nr=it.Count()}).OrderByDescending(s => s.MilliSeconds).Take(10).ToArray();
            foreach (var item in totalTime)
            {
                Console.WriteLine($"!!!!!Method {item.Key} total duration seconds {item.MilliSeconds / 1000} number runs: {item.nr}");
            }
            string x = "";
        }
    }
}
