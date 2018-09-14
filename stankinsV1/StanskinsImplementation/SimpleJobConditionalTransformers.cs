using Newtonsoft.Json;
using StankinsInterfaces;
using StringInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StanskinsImplementation
{
    public class SimpleContainObjects
    {
        public SimpleContainObjects()
        {
            //Children = new List<IBaseObjects>();
            Childs = new SimpleTree();
        }
        public IBaseObjects Key { get; set; }
        //List<IBaseObjects> Children { get; set; }
        //SimpleTree childs;
        public SimpleTree Childs
        {
            get;set;
            //foreach (var item in Children)
            //{
            //    if(!childs.ContainsKey(item) )
            //        childs.Add(item);
            //}
            //return childs;
        }
    }
    public class SimpleTree: List<SimpleContainObjects>
    {
        public SimpleContainObjects KeyFor(IBaseObjects key)
        {
            return this.FirstOrDefault(it => it.Key == key);
        }
        public SimpleContainObjects RecursiveKeyFor(IBaseObjects key)
        {
            var data = KeyFor(key);
            if (data != null)
                return data;
            var items = this.Select(it => it.Childs).ToArray();
            foreach (var item in items)
            {
                data = item.RecursiveKeyFor(key);
                if (data != null)
                    return data;
            }
            return null;
        }
        public bool ContainsKey(IBaseObjects key)
        {
            return KeyFor(key) != null;
        }
        public void Add(IBaseObjects key, IBaseObjects child=null)
        {
            var childKey = KeyFor(key);
            if(childKey == null)
            {
                childKey = new SimpleContainObjects();
                childKey.Key = key;
                this.Add(childKey);
            }

            if (child != null)
            {
                //childKey.Children.Add(child);
                childKey.Childs.Add(child);
            }
        }
        
    }
    public class SimpleJobConditionalTransformers : SimpleJobReceiverTransformer
    {
        public SimpleJobConditionalTransformers():base()
        {
            this.association = new SimpleTree();
        }
        public SimpleTree association { get; set; }
        
        public SimpleJobConditionalTransformers AddSender(ISend send)
        {
            association.Add(send, null);
            return this;
        }
        public SimpleJobConditionalTransformers AddTransformer(ITransform transform)
        {
            association.Add(transform, null);
            return this;
        }
        public SimpleJobConditionalTransformers AddFilter(IFilter filter)
        {
            association.Add(filter, null);
            return this;
        }

        public SimpleJobConditionalTransformers Add(IBaseObjects transformParentNode, IBaseObjects senderORTransform=null)
        {
            //if (!association.ContainsKey(transformParentNode))
            //{                
            //    association.Add(transformParentNode);
            //}
            var val = association.RecursiveKeyFor(transformParentNode);
            if (val == null)
            {
                association.Add(transformParentNode);
                val = association.RecursiveKeyFor(transformParentNode);
            }
            if (senderORTransform != null)
            {
                //val.Children.Add(senderORTransform);
                val.Childs.Add(senderORTransform);
            }
            return this;
        }

        public override async Task Execute()
        {

            var arv = new SyncReceiverMultiple(Receivers.Select(it => it.Value).ToArray());
            await arv.LoadData();
            IRow[] data = arv.valuesRead;
            await TransformAndSendData(association, data);
        }
        async Task<IRow[]> GetDataFromTransform(ITransform transform)
        {
            await transform.Run();
            return transform.valuesTransformed;
        }
        async Task<IRow[]> GetDataFromFilter(IFilter filter)
        {
            await filter.Run();
            return filter.valuesTransformed;
        }
        async Task TransformAndSendData(SimpleTree tree, IRow[] data)
        {
            if (tree == null || tree.Count == 0)
                return;
            //foreach (var transformOrFilter in tree)
            for(int i=0;i<tree.Count;i++)
            {
                var transformOrFilter = tree[i];
                var sender = transformOrFilter.Key as ISend;
                if(sender != null)
                {
                    sender.valuesToBeSent = data;
                    await sender.Send();
                    continue;//sender is last...
                }
                var filter = transformOrFilter.Key as IFilter;
                if (filter != null)
                {
                    filter.valuesRead = data;
                    var newData = await GetDataFromFilter(filter);
                    await TransformAndSendData(transformOrFilter.Childs, newData);
                    data = newData;//pass data to the next filter
                    continue;
                }
                var transform = transformOrFilter.Key as ITransform;
                if (transform != null)
                {
                    transform.valuesRead = data;
                    var newData = await GetDataFromTransform(transform);
                    await TransformAndSendData(transformOrFilter.Childs, newData);
                    data = newData;//pass data to the next filter
                    continue;
                }
                var receiver = transformOrFilter.Key as IReceive;
                if(receiver != null)
                {
                    await receiver.LoadData();
                    var newData = receiver.valuesRead;
                    await TransformAndSendData(transformOrFilter.Childs, newData);
                    data = newData;//pass data to the next filter
                    continue;
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
            settings.Converters.Add(new JsonEncodingConverter());
            var i = new Interpret();
            var newText = i.InterpretText(serializeData);
            var sj = (SimpleJobConditionalTransformers)JsonConvert.DeserializeObject(newText, settings);
            this.association = sj.association;
            this.Receivers = sj.Receivers;
           
        }

        public SimpleJobConditionalTransformers AddReceiver(IReceive receiver)
        {
            this.Receivers.Add(0, receiver);
            return this;
        }
    }
}
