using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using StankinsInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    /// <summary>
    /// override process line
    /// </summary>    
    public abstract class ReceiverFileFromStorage<T> : IReceive<T>
        where T:IComparable<T>
        
    {
        public string Name { get; set; }
        public string FileToRead { get; set; }
        public Encoding FileEnconding { get; set; }
        public T LastValue { get; set; }
        public bool ReadAllFirstTime { get; set; }
        public ReceiverFileFromStorage(string fileToRead,  bool readAllFirstTime,Encoding fileEnconding )
        {
            
            FileToRead = fileToRead;
            FileEnconding = fileEnconding;
            ReadAllFirstTime = readAllFirstTime;
            Name=$"read file {Path.GetFileName(fileToRead)}";
        }
        public IRowReceive[] valuesRead { get; protected set; }
        protected abstract Task ProcessText(string text);
        public event EventHandler StartReadFile;
        public event EventHandler EndReadFile;
        
        protected bool ContinueRead;
        public  async Task LoadData()
        {
            ContinueRead = true;
            StartReadFile?.Invoke(this, EventArgs.Empty);
            using (var stream = File.Open(FileToRead, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream, FileEnconding))
                {
                    if (this.ReadAllFirstTime)
                    {
                        var allText = await reader.ReadToEndAsync();
                        await ProcessText(allText);
                    }
                    else
                    {

                        string line;
                        while (ContinueRead && ((line = await reader.ReadLineAsync()) != null))
                        {
                            
                            try
                            {
                                //maybe process async?!
                                await ProcessText(line);
                            }
                            catch (Exception ex)
                            {
                                string s = ex.Message;
                                //@class.Log(LogLevel.Error,0,"end send data ERROR receiver file from storage"+s,ex,null);
                                throw;
                            }
                        }
                    }
                }


                EndReadFile?.Invoke(this, EventArgs.Empty);
            }
        }
        public void ClearValues()
        {
            valuesRead = null;
        }
    }
}
