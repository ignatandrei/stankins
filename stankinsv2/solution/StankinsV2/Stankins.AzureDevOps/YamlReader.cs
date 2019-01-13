using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace Stankins.AzureDevOps
{
    public class YamlReader : Receiver
    {
        private readonly string fileName;
        private readonly Encoding encoding;

        public YamlReader(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.fileName = GetMyDataOrThrow<string>(nameof(fileName));
            this.encoding = GetMyDataOrDefault(nameof(encoding), Encoding.UTF8);
        }
        public YamlReader(string fileName, Encoding encoding) :this(new CtorDictionary()
        {
            {
                nameof(fileName),fileName
            },
            {
                nameof(encoding),encoding
            }
        })
        {
            this.encoding = encoding;
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData = new DataToSentTable();

            }

            var file = new ReadFileToString
            {
                FileEnconding = this.encoding,
                FileToRead = this.fileName
            };

            var data = await file.LoadData();
            var yaml = new YamlDevOpsVisitor();
            yaml.LoadFromString(data);
            var jobs = yaml.jobs;
            var dt = new DataTable("jobs" );
            dt.Columns.Add(new DataColumn("name", typeof(string)));
            dt.Columns.Add(new DataColumn("condition", typeof(string)));
            
            foreach(var job in jobs)
            {
                dt.Rows.Add(job.Name, job.condition);
            }
            var id = receiveData.AddNewTable(dt);
            receiveData.Metadata.AddTable(dt, id);

            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
