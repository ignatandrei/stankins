using System;
using System.Diagnostics;

namespace StankinsStatusWeb
{
    [DebuggerDisplay("{CRON} {LastRunTime} {NextRunTime}" )]
    public class CronExecutionBase
    {
        public string CRON { get; set; }


        public DateTime? LastRunTime { get; set; }
        public DateTime? NextRunTime { get; set; }
        public void CopyFrom(CronExecutionBase c)
        {
            this.CRON = c.CRON;
            this.LastRunTime = c.LastRunTime;
            this.NextRunTime = c.NextRunTime;
            
        }
    }
}
