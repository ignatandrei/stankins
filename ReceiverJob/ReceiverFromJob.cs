using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        IRowReceive[] FromSimpleJob(ISimpleJob sj)
        {
            var li = new List<IRowReceiveHierarchicalParent>();
            for (int i = 0; i < sj.Receivers.Count; i++)
            {
                var item = sj.Receivers[i];
                var newRow = new RowReadHierarchical();
                newRow.Values.Add("Name", item.Name ?? item.GetType().Name);
                newRow.Values.Add("Type", item.GetType().Name);
                newRow.Values.Add("RowType", "Receiver");
                li.Add(newRow);//TODO: Add parents!
                if (li.Count > 1)
                {
                    newRow.Parent = li[li.Count - 2];
                }
            }

            for (int i = 0; i < sj.FiltersAndTransformers.Count; i++)
            {
                var item = sj.FiltersAndTransformers[i];
                var newRow = new RowReadHierarchical();
                newRow.Values.Add("Name", Name ?? item.GetType().Name);
                newRow.Values.Add("Type", item.GetType().Name);
                newRow.Values.Add("RowType", "Filter_Transformer");
                li.Add(newRow);
                if (li.Count > 1)
                {
                    newRow.Parent = li[li.Count - 1];
                }
            }
            for (int i = 0; i < sj.Senders.Count; i++)
            {
                var item = sj.Senders[i];
                var newRow = new RowReadHierarchical();
                newRow.Values.Add("Name", Name ?? item.GetType().Name);
                newRow.Values.Add("Type", item.GetType().Name);
                newRow.Values.Add("RowType", "Sender");
                li.Add(newRow);
                if (li.Count > 1)
                {
                    newRow.Parent = li[li.Count - 1];
                }
            }
            return li.ToArray();
        }
        public async Task LoadData()
        {
            var job = Job as SimpleJobConditionalTransformers;
            if (job != null)
            {
                this.valuesRead = FromSimpleJobConditionalTransformers(job);
                return;
            }

            ISimpleJob sj = Job as ISimpleJob;
            if(sj != null)
            {
                this.valuesRead = FromSimpleJob(sj);
                return;
            }

            
            //TODO:log
            await Task.CompletedTask;
        }
        IRowReceiveHierarchicalParent[]  FromSimpleTree(SimpleTree st,IRowReceiveHierarchicalParent parent)
        {
            var li = new List<IRowReceiveHierarchicalParent>();
            foreach(var node in st)
            {
                var item = node.Key;
                var newRow = new RowReadHierarchical();
                newRow.Values.Add("Name", item.Name ?? item.GetType().Name);
                newRow.Values.Add("Type", item.GetType().Name);
                ISend send = item as ISend;
                if (send != null)
                    newRow.Values.Add("RowType", "Sender");
                else
                {
                    ITransform tr = item as ITransform;
                    if (tr != null)
                        newRow.Values.Add("RowType", "Filter_Transformer");
                    else
                    {
                        IReceive r = item as IReceive;
                        if (r != null)
                            newRow.Values.Add("RowType", "Receiver");
                        else
                        {
                            //TODO:log
                            Debug.Assert(false);
                        }
                    }

                }
                newRow.Parent = parent;
                li.Add(newRow);
                var childs = node.Childs;
                li.AddRange(FromSimpleTree(childs, newRow));
            }
            return li.ToArray();
        }
        private IRowReceive[] FromSimpleJobConditionalTransformers(SimpleJobConditionalTransformers sj)
        {
            var li = new List<IRowReceiveHierarchicalParent>();
            for (int i = 0; i < sj.Receivers.Count; i++)
            {
                var item = sj.Receivers[i];
                var newRow = new RowReadHierarchical();
                newRow.Values.Add("Name", item.Name ?? item.GetType().Name);
                newRow.Values.Add("Type", item.GetType().Name);
                newRow.Values.Add("RowType", "Receiver");
                li.Add(newRow);
                if (li.Count > 1)
                {
                    newRow.Parent = li[li.Count - 2];
                }
            }
            li.AddRange(FromSimpleTree(sj.association,li[li.Count-1]));
            return li.ToArray();
        }
    }
}
