using StankinsInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StringInterpreter;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Reflection;

namespace StanskinsImplementation
{
    public class JsonEncodingConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var webName = (value as Encoding).WebName;
            serializer.Serialize(writer, webName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var webName = "";
            if (reader.TokenType == JsonToken.String) { 

                webName = reader.Value?.ToString();
            }
            //handling old data format for encoding
            if (reader.TokenType==JsonToken.StartObject)
            {
                webName = reader.Value?.ToString();
                
                while(reader.TokenType != JsonToken.EndObject)
                {
                    if (!reader.Read())
                        break;
                    if (reader.TokenType != JsonToken.PropertyName)
                        continue;
                    var val= reader.Value?.ToString();
                    if (string.Compare("webname", val,StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        webName = reader.ReadAsString();
                        //do not break - advance reading to the end
                        //break;
                    }
                }
                
            }

            existingValue = Encoding.GetEncoding(webName);

            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return (typeof(Encoding).IsAssignableFrom(objectType));
        }
    }
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
            settings.Converters.Add(new JsonEncodingConverter());
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
            await Task.WhenAll(rec.AllTasksWithLogging());
            if (!rec.Exists(it => it.IsSuccess()))
            {
                string noData = "no data for send";
                //@class.Log(LogLevel.Error,0,noData,null,null);
                return;
            }
            var failed = rec.Where(it => !it.IsSuccess()).ToArray();

            foreach (var item in failed)
            {
                string ex = item.Exception.Message;
                //@class.Log(LogLevel.Error,0,$"error in send",item.Exception,null);
                ex += ";";

            }



        }
        public bool AllReceiversAsync { get; set; }
        public bool AllSendersAsync{ get; set; }

        public RuntimeParameter[] RuntimeParameters { get; set; }
        Dictionary<RuntimeParameter, string> variables = new Dictionary<RuntimeParameter, string>();
        public override async Task Execute()
        {
            IReceive arv =null;
            if (Receivers?.Count == 1)
            {
                arv = Receivers[0];
            }
            if (arv == null)
            {
                if (AllReceiversAsync)
                {
                    arv = new AsyncReceiverMultiple(Receivers.Select(it => it.Value).ToArray());
                }
                else
                {
                    arv = new SyncReceiverMultiple(Receivers.Select(it => it.Value).ToArray());
                }
            }
            await arv.LoadData();
            bool existsVar = (RuntimeParameters?.Length ?? 0) > 0;
            string[] nameObjectsWithVariables=null;
            if(existsVar)
                nameObjectsWithVariables = RuntimeParameters
                .SelectMany(it => it.NameObjectsToApplyTo)
                .Select(it=>it.NameObjectToApplyTo.ToLowerInvariant())
                .Distinct()
                .ToArray();
            IRow[] data = arv.valuesRead;
            foreach (var filterKV in FiltersAndTransformers)
            {
                var var = filterKV.Value as TransformIntoVariable;
                if(var != null)
                {
                    if (!existsVar)
                    {
                        //TODO:log
                        continue;
                    }
                    var param = RuntimeParameters.FirstOrDefault(it => it.VariableName == var.VariableName);                    
                    if(param == null)
                    {
                        throw new ArgumentException($"in runtime parameters I cannot find variable {var.VariableName}");
                    }
                    await var.Run();
                    variables[param]= var.Result;
                    continue;
                }
                bool hasVar =(nameObjectsWithVariables?.Length>0) &&(nameObjectsWithVariables.Contains(filterKV.Value.Name.ToLowerInvariant()));
                if (hasVar)
                {
                    TransformPropertyFromVar(filterKV.Value);
                }
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
                if (filter != null)
                {
                    filter.valuesRead = data;
                    await filter.Run();
                    data = filter.valuesTransformed;
                    continue;
                }
                Debug.Assert(false,"base object is not found");
                //@class.Log(LogLevel.Error,0,$"base object is not found",null,null);
                Debug.Assert(false, "filter is not found");

            }
            //await SenderData(data);
            if (Senders.Count == 0)
                return;

            ISend send = null;
            if(Senders.Count == 1)
            {
                send = Senders[0];
            }
            if (send == null)
            {
                if (AllSendersAsync)
                {
                    send = new ASyncSenderMultiple(Senders.Select(it => it.Value).ToArray());
                }
                else
                {
                    send = new SyncSenderMultiple(Senders.Select(it => it.Value).ToArray());
                }
            }
            send.valuesToBeSent = data;
            await send.Send();
        }

        private void TransformPropertyFromVar(IFilterTransformer value)
        {
            var name = value.Name.ToLowerInvariant();
            var runtimes = RuntimeParameters
                .Where(it=>
                it
                .NameObjectsToApplyTo
                .Select(n=>n.NameObjectToApplyTo.ToLowerInvariant())
                .Contains(name))
                .ToArray();

            var props = value.GetType().GetProperties(BindingFlags.Public);
            foreach (var rt in runtimes)
            {
                var prop = props.FirstOrDefault(it => it.Name.ToLowerInvariant() == rt.VariableName.ToLowerInvariant());
                object var = Convert.ChangeType(variables[rt], prop.PropertyType);
                prop.SetValue(value, var);
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
            var newText= i.InterpretText(serializeData);
            var sj=(SimpleJob) JsonConvert.DeserializeObject(newText, settings);
            this.FiltersAndTransformers = sj.FiltersAndTransformers;
            this.Receivers = sj.Receivers;
            this.Senders = sj.Senders;

        }
        #region easy to use
        public SimpleJob AddReceiver(IReceive r)
        {
            this.Receivers.Add(Receivers.Count, r);
            return this;
        }
        public SimpleJob AddSender(ISend s)
        {
            this.Senders.Add(Senders.Count, s);
            return this;
        }
        public SimpleJob AddFilter(IFilter f)
        {
            this.FiltersAndTransformers.Add(FiltersAndTransformers.Count, f);
            return this;
        }
        public SimpleJob AddTransformer(IFilterTransformer t)
        {
            this.FiltersAndTransformers.Add(FiltersAndTransformers.Count, t);
            return this;
        }
        #endregion
    }
}
