using Newtonsoft.Json;
using ReceiverFile;
using StankinsInterfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverJSON
{
    //TODO: make a plain ReceiverFileFromStorage
    public class ReceiverJSONFileInt: ReceiverFileFromStorage<int>         
    {
        public ReceiverJSONFileInt(string fileToRead, Encoding fileEnconding):base(fileToRead, true, fileEnconding)
        {
            Name = "read json from " + fileToRead;
        }
        protected override async Task ProcessText(string text)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                //Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };
            var data = JsonConvert.DeserializeObject<IRowReceive[]>(text, settings);
            valuesRead = data;

        }
    }
}
