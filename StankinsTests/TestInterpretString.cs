using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StringInterpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StankinsTests
{
    [TestClass]
    public class TestInterpretString
    {
        
        public static string RandomString(int length)
        {
            var random = new Random(DateTime.Now.Second);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [TestMethod]
        public async Task InterpretTextNone()
        {
            string textToInterpret = RandomString(22);

            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            Assert.AreEqual(textToInterpret, str);

        }
        [TestMethod]
        public async Task InterpretSettingsFile()
        {
            string textToInterpret = "this is from #file:SqlServerConnectionString#";
            
            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            Assert.IsFalse(str.Contains("#"));
            Assert.IsTrue(str.Contains("this is from"));
            Assert.IsTrue(str.Contains("Database"));
            Assert.IsTrue(str.Contains("Trusted_Connection"));
            await Task.CompletedTask;

        }
        [TestMethod]
        public async Task InterpretEnv()
        {
            
            string textToInterpret = "";
            string textInterpreted = "";
            var var = Environment.GetEnvironmentVariables();
            foreach (var item in var.Keys)
            {
                var randomString = RandomString(10);
                textToInterpret += randomString + "#env:" + item + "#" + Environment.NewLine;
                textInterpreted += randomString + var[item]  + Environment.NewLine;
                continue;
            }
            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            Assert.AreEqual(textInterpreted, str);
            await Task.CompletedTask;


        }
    }
}

