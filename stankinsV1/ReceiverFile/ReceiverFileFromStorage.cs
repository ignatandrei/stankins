using StankinsInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ReceiverFile
{
    
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
        bool IsLocalFile()
        {
            try
            {
                var uri = new Uri(FileToRead, UriKind.RelativeOrAbsolute);
                return uri.IsFile;
            }
            catch (Exception)
            {
                //if it is local file, 
                //then uri.IsFile will send error
                return true;
            }
        }
        public async Task LoadData()
        {

            var b = IsLocalFile();
            if(b)
            {
                await LoadLocalFile();
                return;
            }
            //is a file on internet
            using (var wc = new WebClient())
            {
                StartReadFile?.Invoke(this, EventArgs.Empty);
                var allText = await wc.DownloadStringTaskAsync(FileToRead);
                await ProcessText(allText);
                EndReadFile?.Invoke(this, EventArgs.Empty);
            }
        }
        async Task LoadLocalFile()
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
