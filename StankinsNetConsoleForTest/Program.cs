using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsNetConsoleForTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string NameFile = "fileWithData.csv";
            if (File.Exists(NameFile))
                File.Delete(NameFile);
            Func<bool> VerifyAnswers = () => VerifyCSV(NameFile);

            if (!SuccessRunFile("OleDBTestToCSV.txt", VerifyAnswers))
            {
                Environment.Exit(-1);
            }
        }
        
        static bool VerifyCSV(string nameFile)
        {
            bool existsFile = File.Exists(nameFile);
            if (!existsFile)
            {
                Console.WriteLine("file export csv does not exists");
                return false;
            }
            FileInfo fi = new FileInfo(nameFile);
            return (fi.Length > 1000);
            
        }
        static bool SuccessRunFile(string fileNameWithJob, Func<bool> VerifyAnswers)
        {
            //var p = Process.Start("StankinsSimpleJobNET.exe", "execute " + fileNameWithJob);
            //p.WaitForExit();
            var exitCode = StankinsSimpleJob.Program.ExecuteJob(fileNameWithJob);
            //if (p.ExitCode == 0)
            if(exitCode == 0)
            {
                Console.WriteLine("executed successfully");
                var v= VerifyAnswers();
                Console.WriteLine(" verified answers: " + v);
                return v;
                
            }
            Console.WriteLine("Test :" + fileNameWithJob + "has an error");
            return false;
        }
    }
}
