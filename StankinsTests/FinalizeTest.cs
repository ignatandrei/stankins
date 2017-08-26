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
            var time= data.GroupBy(it => it.Key).Select(it => new { it.Key, MilliSeconds = it.Sum(t => t.duration.TotalMilliseconds)/it.Count() }).OrderByDescending(s=>s.MilliSeconds).Take(10).ToArray();
            foreach(var item in time)
            {
                Console.WriteLine($"!!!!!Method {item.Key} duration seconds {item.MilliSeconds/1000}");
            }
            string x = "";
        }
    }
}
