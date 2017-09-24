using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StanskinsImplementation
{
    /// <summary>
    /// TODO: logging if one receiver does not work
    /// </summary>
    public class AsyncReceiverMultiple: IReceive
    {
        public string Name { get; set; }
        public IReceive[] Receivers { get; set; }

        public IRowReceive[] valuesRead { get; set; }

        public AsyncReceiverMultiple(params IReceive[] receivers)
        {
            this.Receivers = receivers;
        }
        async Task<IRowReceive[]> DataFromReceivers()
        {
            List<IRowReceive> data = new List<IRowReceive>();
            var rec = new TaskExecutedList();
            for (int i = 0; i < Receivers.Length; i++)
            {
                var te = new TaskExecuted(Receivers[i].LoadData());
                rec.Add(te);

            }
            await Task.WhenAll(rec.AllTasksWithLogging());
            if (!rec.Exists(it => it.IsSuccess()))
            {
                //LOG: no data
                return null;
            }
            var failed = rec.Select(it => !it.IsSuccess()).ToArray();

            foreach (var item in failed)
            {
                //LOG: exception why failed
            }

            for (int i = 0; i < Receivers.Length; i++)
            {
                var item = Receivers[i];
                if (item.valuesRead?.Length == 0)
                    continue;
                data.AddRange(item.valuesRead);

            }
            return data.ToArray();
        }
        public async Task LoadData()
        {
            valuesRead= await DataFromReceivers();
            
        }
    }
}
