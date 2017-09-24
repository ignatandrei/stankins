using System.Threading.Tasks;
using StankinsInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace StanskinsImplementation
{
    public class SyncSenderMultiple : ISend
    {
        public string Name { get; set; }
        public ISend[] Senders { get; set; }

        public IRow[] valuesToBeSent { get; set; }

        public SyncSenderMultiple(params ISend[] senders)
        {
            this.Senders = senders;
            
        }

        public async Task Send()
        {
            var data = new List<IRow>();
            
            for (int i = 0; i < Senders.Length; i++)
            {
                Senders[i].valuesToBeSent = valuesToBeSent;
                var te = new TaskExecuted(Senders[i].Send());
                await te.t;
                if(!te.IsSuccess())
                {
                    //TODO: log
                }
            }
            

        }
    }
    public class ASyncSenderMultiple : ISend
    {
        public string Name { get; set; }
        public ISend[] Senders { get; set; }

        public IRow[] valuesToBeSent { get; set; }

        public ASyncSenderMultiple(params ISend[] senders)
        {
            this.Senders= senders;
        }

        public async Task Send()
        {
            var data = new List<IRow>();
            var rec = new TaskExecutedList();
            for (int i = 0; i < Senders.Length; i++)
            {
                Senders[i].valuesToBeSent = valuesToBeSent;
                var te = new TaskExecuted(Senders[i].Send());
                rec.Add(te);

            }
            //await Task.WhenAll(rec.AllTasksWithLogging());
            await Task.WhenAll(rec.AllTasksWithLogging());
            if (!rec.Exists(it => it.IsSuccess()))
            {
                //LOG: no data
                return;
            }
            var failed = rec.Select(it => !it.IsSuccess()).ToArray();

            foreach (var item in failed)
            {
                //LOG: exception why failed
            }

        }
    }
}
