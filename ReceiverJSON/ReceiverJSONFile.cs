using Newtonsoft.Json;
using ReceiverFile;
using StankinsInterfaces;
using StanskinsImplementation;
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
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException($"cannot deserialize empty text from {FileToRead}");
            }
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                //Error = HandleDeserializationError
                ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };
            settings.Converters.Add(new JsonEncodingConverter());
            var data = JsonConvert.DeserializeObject<IRowReceive[]>(text, settings);
            valuesRead = data;
            await Task.CompletedTask;

        }
    }
}
