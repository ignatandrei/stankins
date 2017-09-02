using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsNetConsoleForTest
{
    class Program
    {
        static void Main(string[] args)
        {
        }
        static bool SuccessRunFile(string fileNameWithJob)
        {
            var p = Process.Start("StankinsSimpleJobNET.exe", "execute " + fileNameWithJob);
            p.WaitForExit();
            if (p.ExitCode == 0)
                return true;
            Console.WriteLine("Test :" + fileNameWithJob + "has an error");
            return false;
        }
    }
}
