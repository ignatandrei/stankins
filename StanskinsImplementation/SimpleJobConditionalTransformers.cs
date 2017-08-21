using Newtonsoft.Json;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StanskinsImplementation
{
    public class SimpleTree<K, V> : Dictionary<K, SimpleTree<K, V>>
    {
        public V Value { get; set; }
    }

    public class SimpleJobConditionalTransformers : SimpleJobReceiverTransformer
    {
        public SimpleJobConditionalTransformers():base()
        {
            this.association = new SimpleTree<IBaseObjects, IBaseObjects>();
        }
        public SimpleTree<IBaseObjects, IBaseObjects> association { get; set; }
        
        public void AddSender(ISend send)
        {
            association.Add(send, null);
        }
        
        public void Add(IBaseObjects transformParentNode, IBaseObjects senderORTransform=null)
        {
            if (!association.ContainsKey(transformParentNode))
            {                
                association.Add(transformParentNode, new SimpleTree<IBaseObjects, IBaseObjects>());
            }
            if (senderORTransform != null)
            {
                var val = association[transformParentNode];
            
                val.Add(senderORTransform, new SimpleTree<IBaseObjects, IBaseObjects>());
            }
        }

        public override async Task Execute()
        {
            var data = await DataFromReceivers();

            await TransformAndSendData(association, data);




        }
        async Task<IRow[]> GetDataFromFilter(ITransform filter)
        {
            await filter.Run();
            return filter.valuesTransformed;
        }
        async Task TransformAndSendData(SimpleTree<IBaseObjects, IBaseObjects> tree, IRow[] data)
        {
            if (tree == null || tree.Count == 0)
                return;
            foreach (var transformOrFilter in tree)
            {
                var sender = transformOrFilter.Key as ISend;
                if(sender != null)
                {
                    sender.valuesToBeSent = data;
                    await sender.Send();
                    continue;//sender is last...
                }
                var filter = transformOrFilter.Key as ITransform;
                if (filter != null)
                {
                    filter.valuesRead = data;
                    var newData = await GetDataFromFilter(filter);
                    await TransformAndSendData(transformOrFilter.Value, newData);
                    data = newData;//pass data to the next filter
                }

               
                
                
            }
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
            var sj = (SimpleJobConditionalTransformers)JsonConvert.DeserializeObject(serializeData, settings);
            this.association = sj.association;
           
        }

    }
}
