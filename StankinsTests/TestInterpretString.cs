using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StringInterpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Data.SqlClient;
using System.Data;

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
        public void InterpretTextNone()
        {
            #region arrange
            string textToInterpret = RandomString(22);
            #endregion
            #region act

            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Assert.AreEqual(textToInterpret, str);
            #endregion
        }
        [TestMethod]
        public void InterpretSettingsFile()
        {
            #region arrange
            string textToInterpret = "this is from #file:SqlServerConnectionString#";
            #endregion
            #region act

            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Console.WriteLine("interpreted: " + str);
            Assert.IsFalse(str.Contains("#"),"should be interpreted");
            Assert.IsTrue(str.Contains("this is from"),"should contain first chars");
            //Assert.IsTrue(str.Contains("atabase"),"should contain database");
            Assert.IsTrue(str.Contains("rusted"),"should containt trusted connection");
            #endregion
            

        }
        [TestMethod]
        public void InterpretEnv()
        {
            #region arrange
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
            #endregion
            #region act
            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Assert.AreEqual(textInterpreted, str);
            
            #endregion
            

        }
        [TestMethod]
        public void InterpretDateTime()
        {
            #region arrange
            string textToInterpret = "this is from #now:yyyyMMddHHmmss#";
            #endregion
            #region act

            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Console.WriteLine("interpreted: " + str);
            Assert.IsTrue(str.Contains("this is from "+ DateTime.Now.ToString("yyyyMMdd")), "should contain date");            
            #endregion
        }
        [TestMethod]
        public async Task RunJobInterpreted()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;
            File.WriteAllText(Path.Combine(dir, "a.csv"), "asdasdasd");
            Assert.IsTrue(File.Exists(Path.Combine(dir, "a.csv")));
            foreach (var item in Directory.EnumerateFiles(dir, "*.csv"))
            {
                File.Delete(item);
            }
            Assert.IsFalse(File.Exists(Path.Combine(dir, "a.csv")));
            #endregion
            //#region act

            //#endregion
            //#region assert
            //#endregion

        }
    }
}

