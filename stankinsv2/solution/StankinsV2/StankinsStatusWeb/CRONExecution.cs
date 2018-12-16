using Cronos;
using StankinsObjects;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    
    public abstract class CRONExecution: CronExecutionBase , IToBaseObjectExecutable
    {

        public CustomData CustomData { get; set; }

        public abstract BaseObject baseObject();

        public async Task<DataTable> Execute()
        {

            var ret = await baseObject().TransformData(null);
            return ret.DataToBeSentFurther.Values.First();
        }

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
