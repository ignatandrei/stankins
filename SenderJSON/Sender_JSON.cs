using Newtonsoft.Json;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenderJSON
{
    public class Sender_JSON : ISend
    {


        public IRow[] valuesToBeSent { private get; set; }
        public string JSONFileName { get; set; }
        public Sender_JSON(string fileName)
        {
            this.JSONFileName = fileName;
        }


        public async Task Send()
        {


            if (valuesToBeSent?.Length == 0)
            {
                //LOG: there are no data 
                return;
            }
            var nrValues = valuesToBeSent.LongCount();
            var dict = valuesToBeSent
                //.SelectMany(it => it.Values).ToArray();
                .Select(it => it.Values).ToArray();
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                //Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };

            var data = JsonConvert.SerializeObject(dict, settings);
            File.WriteAllText(JSONFileName, data);

        }
    }
}
