using MediatR;
using Stankins.Alive;

namespace StankinsStatusWeb
{
    public class ResultWithData : INotification
    {
        public AliveResult AliveResult { get; set; }
        public CustomData CustomData { get; set; }
        public CronExecutionBase CRONExecution { get; set; }
    }
}
