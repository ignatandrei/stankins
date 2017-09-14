using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransform
{
    public class MediaTransformJSON : MediaTransformString
    {
        public override async Task Run()
        {
            if (valuesToBeSent?.Length == 0)
            {
                //LOG: there are no data 
                return;
            }
            var nrValues = valuesToBeSent.LongCount();
            var dict = valuesToBeSent;
                //.SelectMany(it => it.Values).ToArray();
                //.Select(it => it.Values).ToArray();
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                //Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };

            var data = JsonConvert.SerializeObject(dict, settings);
            Result = data;
            await Task.CompletedTask;
        }
    }
}
