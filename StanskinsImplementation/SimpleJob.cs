using StankinsInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StringInterpreter;
using System.Diagnostics;

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
        public bool AllReceiversAsync { get; set; }
        public bool AllSendersAsync{ get; set; }
        public override async Task Execute()
        {
            IReceive arv;
            if (AllReceiversAsync)
            {
                arv = new AsyncReceiverMultiple(Receivers.Select(it => it.Value).ToArray());
            }
            else
            {
                arv=new SyncReceiverMultiple(Receivers.Select(it => it.Value).ToArray());
            }
            await arv.LoadData();
            IRow[] data = arv.valuesRead;
            foreach (var filterKV in FiltersAndTransformers)
            {
                //TODO: see also IFilterTransformer
                var transform = filterKV.Value as ITransform;
                if (transform != null)
                {
                    transform.valuesRead = data;
                    await transform.Run();
                    data = transform.valuesTransformed;
                    continue;
                }
                var filter = filterKV.Value as IFilter;
                if (filter == null)
                {
                    filter.valuesRead = data;
                    await filter.Run();
                    data = filter.valuesTransformed;
                    continue;
                }
                //TODO: log
                Debug.Assert(false);

            }
            //await SenderData(data);
            ISend send;
            if (AllSendersAsync)
            {
                send = new ASyncSenderMultiple(Senders.Select(it => it.Value).ToArray());
            }
            else
            {
                send = new SyncSenderMultiple(Senders.Select(it => it.Value).ToArray());
            }
            send.valuesToBeSent = data;
            await send.Send();
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
            var i = new Interpret();
            var newText= i.InterpretText(serializeData);
            var sj=(SimpleJob) JsonConvert.DeserializeObject(newText, settings);
            this.FiltersAndTransformers = sj.FiltersAndTransformers;
            this.Receivers = sj.Receivers;
            this.Senders = sj.Senders;

        }
    }
}
