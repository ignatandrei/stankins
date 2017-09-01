using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransform
{
    public class MediaTransformBSON: MediaTransformByte
    {
        public override async Task Run()
        {
            if (valuesToBeSent?.Length == 0)
            {
                //LOG: there are no data 
                return;
            }
            var nrValues = valuesToBeSent.LongCount();
            //var dict = valuesToBeSent
            //    //.SelectMany(it => it.Values).ToArray();
            //    .Select(it => it.Values).ToArray();
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                //Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };
            using (var ms = new MemoryStream())
            {
                using (var writer = new BsonWriter(ms))
                {
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    serializer.Serialize(writer, valuesToBeSent);
                }
                //ms.Position = 0;
                Result = ms.ToArray();
            }
            //var data = JsonConvert.SerializeObject(valuesToBeSent, settings);
            
        }
    }
}
