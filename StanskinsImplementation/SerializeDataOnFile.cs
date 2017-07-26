using Newtonsoft.Json;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StanskinsImplementation
{
    public class SerializeDataOnFile : SerializeDataInMemory, IDisposable
    {
        public string FileName { get; set; }
        bool haveReadFile = false;
        public SerializeDataOnFile(string fileName)
        {
            FileName = fileName;
            
        }
        void ReadAllFile()
        {

            var fileContent = File.ReadAllText(FileName);
            if (!string.IsNullOrWhiteSpace(fileContent)) {
                data = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContent);
            }
            haveReadFile = true;
        }

        public new object GetValue(string key)
        {
            if (!haveReadFile)
            {
                ReadAllFile();
            }
            return base.GetValue(key);
        }

        public new void SetValue(string key, object value)
        {

            if (!haveReadFile)
            {
                ReadAllFile();
            }
            base.SetValue(key,value);
        }
        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //data = null;
                }
                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    Formatting = Formatting.Indented,
                    //Error = HandleDeserializationError
                    //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

                };
                var val = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(FileName, val);
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                val=null;
                data = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~SerializeDataOnFile()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
