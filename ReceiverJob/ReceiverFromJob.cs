using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReceiverJob
{
    public class ReceiverFromJob:IReceive
    {
        public ReceiverFromJob(IJob job)
        {
            Job = job;
        }

        

        public IRowReceive[] valuesRead { get; set; }

        public string Name { get ; set ; }
        public IJob Job { get; }

        public async Task LoadData()
        {
            var li = new List<IRowReceiveHierarchical>();
            ISimpleJob sj = Job as ISimpleJob;
            if(sj != null)
            {
                for (int i = 0; i < sj.Receivers.Count; i++)
                {
                    var item = sj.Receivers[i];
                    var newRow = new RowReadHierarchical();
                    newRow.Values.Add("Name",Name ?? item.GetType().Name);
                    newRow.Values.Add("Type", item.GetType().Name);
                    li.Add(newRow);//TODO: Add parents!
                    newRow.Parent = li[li.Count - 1];

                }
                
                for(int i=0;i<sj.FiltersAndTransformers.Count;i++)
                {
                    var item = sj.FiltersAndTransformers[i];
                    var newRow = new RowReadHierarchical();
                    newRow.Values.Add("Name", Name ?? item.GetType().Name);
                    newRow.Values.Add("Type", item.GetType().Name);
                    li.Add(newRow);
                    newRow.Parent = li[li.Count - 1];
                }
                for (int i = 0; i < sj.Senders.Count; i++)
                {
                    var item = sj.Senders[i];
                    var newRow = new RowReadHierarchical();
                    newRow.Values.Add("Name", Name ?? item.GetType().Name);
                    newRow.Values.Add("Type", item.GetType().Name);
                    li.Add(newRow);
                    newRow.Parent = li[li.Count - 1];
                }

            }
        }
    }
}
