using StankinsInterfaces;
using System;
using System.Threading.Tasks;

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
                //TODO: log
                return;
            }
                
            if (ActionToRow == null)
            {
                //TODO: log
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
