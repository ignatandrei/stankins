using StankinsInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StanskinsImplementation
{
    public class SimpleJob : ISimpleJob
    {
        public OrderedList<IReceive> Receivers { get; set; }

        public OrderedList<IFilterTransformer> FiltersAndTransformers { get; set; }

        public OrderedList<ISend> Senders { get; set; }

        public SimpleJob()
        {
            Receivers = new OrderedList<IReceive>();
            FiltersAndTransformers = new OrderedList<IFilterTransformer>();
            Senders = new OrderedList<ISend>();

        }

        public async Task<IRow[]> DataFromReceivers()
        {
            List<IRow> data = new List<IRow>();
            var rec = new TaskExecutedList();
            for (int i = 0; i < Receivers.Count; i++)
            {
                var te = new TaskExecuted(Receivers[i].LoadData());
                rec.Add(te);

            }
            await Task.WhenAll(rec.AllTasksWithErrors());
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

            for (int i = 0; i < Receivers.Count; i++)
            {
                var item = Receivers[i];
                if (item.valuesRead?.Length == 0)
                    continue;
                data.AddRange(item.valuesRead);

            }
            return data.ToArray();
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
        public async Task Execute()
        {
            var data = await DataFromReceivers();
            foreach (var item in FiltersAndTransformers)
            {
                var itemData = item.Value as ITransform;
                if (itemData != null)
                {
                    itemData.valuesRead = data;
                }
                await item.Value.Run();
                if (itemData != null) { 
                    data = itemData.valuesTransformed;
                }
            }
            await SenderData(data);
        }

        public string SerializeMe()
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

        public void UnSerialize(string serializeData)
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
