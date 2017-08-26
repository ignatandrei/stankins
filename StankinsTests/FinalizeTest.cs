using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            string x = "";
        }
    }
}
