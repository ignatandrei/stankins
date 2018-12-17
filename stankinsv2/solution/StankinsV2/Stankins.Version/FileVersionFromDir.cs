using Newtonsoft.Json;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Stankins.Version
{
    public class FileVersionFromDir : Receiver
    {
        private readonly string dirPath;

        public FileVersionFromDir(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            dirPath = base.GetMyDataOrThrow<string>(nameof(dirPath));

        }
        public FileVersionFromDir(string dirPath) : this(new CtorDictionary()
            {
                {nameof(dirPath),dirPath }

            })
        {
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData = new DataToSentTable();
            }

            DataTable dt = new DataTable
            {
                TableName = Path.GetDirectoryName(dirPath)
            };
            dt.Columns.Add(new DataColumn("FileName", typeof(string)));
            dt.Columns.Add(new DataColumn("FullFileName", typeof(string)));
            dt.Columns.Add(new DataColumn("Info", typeof(string)));

            foreach (string item in Directory.EnumerateFiles(dirPath))
            {
                try
                {
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(item);
                    dt.Rows.Add(new[] { Path.GetFileName(item), item, JsonConvert.SerializeObject(info) });

                }
                catch (Exception)
                {
                    //TODO: log
                }
            }
            int id = receiveData.AddNewTable(dt);
            receiveData.Metadata.AddTable(dt, id);
            return receiveData;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}


