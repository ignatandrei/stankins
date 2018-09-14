using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using StankinsInterfaces;
using System.IO;
using System.Threading.Tasks;
using StanskinsImplementation;

namespace ReceiverFile
{
    public class ReceiverFileFromStorageBinary : IReceive
    {
        public ReceiverFileFromStorageBinary(string fileName)
        {
            this.FileName = fileName;
        }
        public IRowReceive[] valuesRead { get; protected set; }
        public string FileName { get; set; }
        public string Name { get; set; }

        public async Task LoadData()
        {
            using (var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                byte[] buffer = new byte[fs.Length];
                await fs.ReadAsync(buffer, 0, buffer.Length);
                using(var ms = new MemoryStream(buffer))
                {
                    using(var reader = new BsonReader(ms)){
                        reader.ReadRootValueAsArray = true;
                        var settings = new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.Objects,
                            Formatting = Formatting.Indented,
                            //Error = HandleDeserializationError
                            //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

                        };
                        settings.Converters.Add(new JsonEncodingConverter());
                        var des = JsonSerializer.Create(settings);
                        valuesRead=des.Deserialize<IRowReceive[]>(reader);

                    }
                }
            }
        }
        public void ClearValues()
        {
            valuesRead = null;
        }
    }
}
