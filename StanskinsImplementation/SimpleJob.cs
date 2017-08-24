using StankinsInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StanskinsImplementation
{
    public abstract class SimpleJobReceiverTransformer : IJob
    {
        public SimpleJobReceiverTransformer()
        {
            Receivers = new OrderedList<IReceive>();
        }
        public OrderedList<IReceive> Receivers { get; set; }
        
        public abstract Task Execute();
        
        public virtual string SerializeMe()
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                //Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };
            return JsonConvert.SerializeObject(this, settings);
        }

        public abstract void UnSerialize(string serializeData);
        
    }

    public class SimpleJob : SimpleJobReceiverTransformer, ISimpleJob
    {
               
        public OrderedList<ISend> Senders { get; set; }
        public OrderedList<IFilterTransformer> FiltersAndTransformers { get; set; }
        public SimpleJob():base()
        {
            
            FiltersAndTransformers = new OrderedList<IFilterTransformer>();
            Senders = new OrderedList<ISend>();

        }

       
        public async Task SenderData(IRow[] dataToSend)
        {
            List<IRow> data = new List<IRow>();
            var rec = new TaskExecutedList();
            for (int i = 0; i < Senders.Count; i++)
            {
                Senders[i].valuesToBeSent = dataToSend;
                var te = new TaskExecuted(Senders[i].Send());
                rec.Add(te);

            }
            //await Task.WhenAll(rec.AllTasksWithLogging());
            await Task.WhenAll(rec.AllTasksWithErrors());
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
        public override async Task Execute()
        {
            var arv = new SyncReceiverMultiple(Receivers.Select(it=>it.Value).ToArray());
            await arv.LoadData();
            IRow[] data = arv.valuesRead;
            foreach (var filterKV in FiltersAndTransformers)
            {
                var filter = filterKV.Value as ITransform;
                if (filter == null)
                {
                    //TODO: log
                    continue;
                }

                filter.valuesRead = data;
                await filter.Run();
                data = filter.valuesTransformed;                
            }
            await SenderData(data);
        }

        

        public override void UnSerialize(string serializeData)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                //Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };
            var sj=(SimpleJob) JsonConvert.DeserializeObject(serializeData, settings);
            this.FiltersAndTransformers = sj.FiltersAndTransformers;
            this.Receivers = sj.Receivers;
            this.Senders = sj.Senders;

        }
    }
}
