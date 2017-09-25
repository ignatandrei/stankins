using StankinsInterfaces;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace SenderAction
{
    //TODO: make an abstract class with Roslyn to serialize
    public abstract class SenderActionOrdered : ISend
    {
        public IRow[] valuesToBeSent { get; set ; }
        public string Name { get ; set ; }
        protected Action<int,IRow> ActionToRow { get; set; }
        protected SenderActionOrdered():this(null)
        {

        }
        public SenderActionOrdered(Action<int,IRow> action)
        {
            ActionToRow = action;
        }
        public virtual async Task Send()
        {
            if (valuesToBeSent?.Length == 0)
            {
                string message = $"no values to be sent ";
                //@class.Log(LogLevel.Information, 0, $"sender action ordered: {message}", null, null);                        
                message += "";
                return;
            }
                
            if (ActionToRow == null)
            {
                string message = $"actiontorow is null ";
                //@class.Log(LogLevel.Information, 0, $"sender action ordered: {message}", null, null);                        
                message += "";
                return;
            }
            int i=0;                
            foreach(var val in valuesToBeSent)
            {                
                ActionToRow?.Invoke(i++,val);
            }
        }
    }
}
