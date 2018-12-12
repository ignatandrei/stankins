using Cronos;
using System;

namespace StankinsStatusWeb
{
    public class CRONExecution: CronExecutionBase 
    {
       
       

        
        void MakeNextExecute()
        {
            LastRunTime = NextRunTime;
            //todo: cache this
            var expression = CronExpression.Parse(CRON, CronFormat.IncludeSeconds);
            NextRunTime= expression.GetNextOccurrence(DateTime.UtcNow);
        }
        public bool ShouldRun(DateTime currentTime)
        {
            if (NextRunTime == null && LastRunTime == null)
            {
                MakeNextExecute();
                return true;//execute once
            }
            if (NextRunTime == null)
                return false;

            if (NextRunTime < currentTime)
            {
                MakeNextExecute();
                return true;
            }
            return false;
        }
    }
}
