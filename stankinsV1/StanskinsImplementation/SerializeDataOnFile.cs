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

            if (!File.Exists(FileName))
            {
                using (StreamWriter sw = File.CreateText(FileName)) { } //It creates and close file used for testing
            }
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

        public Dictionary<string, object> GetDictionary()
        {
            if (!haveReadFile)
            {
                ReadAllFile();
            }
            return this.data;
        }

        public void SetDictionary(Dictionary<string, object> obj)
        {
            if (!haveReadFile)
            {
                ReadAllFile();
            }
            this.data = obj;
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
                settings.Converters.Add(new JsonEncodingConverter());
                var val = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(FileName, val);
                val=null;
                data = null;

                disposedValue = true;
            }
        }

        ~SerializeDataOnFile()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
