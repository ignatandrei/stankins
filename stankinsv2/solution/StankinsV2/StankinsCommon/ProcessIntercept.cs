using System.Diagnostics;

namespace StankinsCommon
{
    /// <summary>
    /// intercept 
    /// OutputDataReceived
    /// ErrorDataReceived 
    /// Exited
    /// </summary>
    public class ProcessIntercept: Process
    {
        Process process;
        public ProcessIntercept(string fileName, string arguments)
        {
            process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
        }
        public void StartProcessAndWait()
        {
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

    }
}