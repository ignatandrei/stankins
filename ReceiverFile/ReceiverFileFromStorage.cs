using StankinsInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverFile
{
    /// <summary>
    /// override process line
    /// </summary>    
    public abstract class ReceiverFileFromStorage<T> : IReceive<T>
        where T:IEquatable<T>
        
    {

        public string FileToRead { get; set; }
        public Encoding FileEnconding { get; set; }
        public T LastValue { get; set; }
        public bool ReadAllFirstTime { get; set; }
        public ReceiverFileFromStorage(string fileToRead,  bool readAllFirstTime,Encoding fileEnconding )
        {
            
            FileToRead = fileToRead;
            FileEnconding = fileEnconding;
            ReadAllFirstTime = readAllFirstTime;
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
                                //todo: log
                            }
                        }
                    }
                }


                EndReadFile?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
