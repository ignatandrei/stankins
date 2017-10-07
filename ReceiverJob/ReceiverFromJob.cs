using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ReceiverJob
{
    public class ReceiverFromJobFile : ReceiverFromJob
    {
        public ReceiverFromJobFile(string fileName):base(null)
        {
            FileName = fileName;
            Name = $"receiver from job serialized in {fileName}";
        }

        public string FileName { get; set; }
        public override Task LoadData()
        {
            var text = File.ReadAllText(FileName);
            if (!TryGetSimpleJobConditionalTransformers(text))
            {
                if (!TryGetSimpleJob(text))
                {
                    throw new ArgumentException($"cannot deserialize from file : {FileName}");
                }
            }
            return base.LoadData();
        }
        bool TryGetSimpleJobConditionalTransformers(string text)
        {
            try
            {
                var sj = new SimpleJobConditionalTransformers();
                sj.UnSerialize(text);
                Job = sj;
                return true;
            }
            catch (Exception)
            {
                return false;
                
            }
            

        }
        bool TryGetSimpleJob(string text)
        {
            try
            {
                var sj = new SimpleJob();
                sj.UnSerialize(text);
                Job = sj;
                return true;
            }
            catch (Exception)
            {
                return false;

            }


        }

    }
    public class ReceiverFromJob:IReceive
    {
        public ReceiverFromJob(IJob job)
        {
            Job = job;
        }

        

        public IRowReceive[] valuesRead { get; set; }

        public string Name { get ; set ; }
        public IJob Job;
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
                newRow.Values.Add("Name", item.Name ?? item.GetType().Name);
                newRow.Values.Add("Type", item.GetType().Name);
                newRow.Values.Add("RowType", "Filter_Transformer");
                li.Add(newRow);
                if (li.Count > 1)
                {
                    newRow.Parent = li[li.Count - 2];
                }
            }
            for (int i = 0; i < sj.Senders.Count; i++)
            {
                var item = sj.Senders[i];
                var newRow = new RowReadHierarchical();
                newRow.Values.Add("Name", item.Name ?? item.GetType().Name);
                newRow.Values.Add("Type", item.GetType().Name);
                newRow.Values.Add("RowType", "Sender");
                li.Add(newRow);
                if (li.Count > 1)
                {
                    newRow.Parent = li[li.Count - 2];
                }
            }
            return li.ToArray();
        }
        public virtual async Task LoadData()
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
            foreach (var node in st)
            {
                var item = node.Key;
                var newRow = new RowReadHierarchical();
                newRow.Values.Add("Name", item.Name ?? item.GetType().Name);
                newRow.Values.Add("Type", item.GetType().Name);
                newRow.Parent = parent;
                li.Add(newRow);
                var childs = node.Childs;
                li.AddRange(FromSimpleTree(childs, newRow));

                ISend send = item as ISend;
                if (send != null) {
                    newRow.Values.Add("RowType", "Sender");
                    continue;
                }
                ITransform tr = item as ITransform;
                if (tr != null)
                {
                    newRow.Values.Add("RowType", "Transformer");
                    continue;
                }
                IFilter fi = item as IFilter;
                if (fi != null)
                {
                    newRow.Values.Add("RowType", "Filter");
                    continue;
                }
                IReceive r = item as IReceive;
                if (r != null)
                {
                    newRow.Values.Add("RowType", "Receiver");
                    continue;
                }
                

            
              
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
        public void ClearValues()
        {
            valuesRead = null;
        }
    }
}
