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
        
        public ProcessIntercept(string fileName, string arguments):base()
        {
            
            this.EnableRaisingEvents = true;
            this.StartInfo.FileName = fileName;
            this.StartInfo.Arguments = arguments;
            this.StartInfo.UseShellExecute = false;
            this.StartInfo.RedirectStandardError = true;
            this.StartInfo.RedirectStandardOutput = true;
        }
        public void StartProcessAndWait()
        {
            this.Start();
            this.BeginErrorReadLine();
            this.BeginOutputReadLine();
            this.WaitForExit();
        }

    }
}