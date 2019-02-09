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
        private readonly string fileNameYaml;
        private readonly Encoding encoding;

        public YamlReader(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.fileNameYaml = GetMyDataOrThrow<string>(nameof(fileNameYaml));
            this.encoding = GetMyDataOrDefault(nameof(encoding), Encoding.UTF8);
            this.Name = nameof(YamlReader);
        }
        public YamlReader(string fileNameYaml, Encoding encoding) :this(new CtorDictionary()
        {
            {
                nameof(fileNameYaml),fileNameYaml
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
                FileToRead = this.fileNameYaml
            };

            var data = await file.LoadData();
            var yaml = new YamlDevOpsVisitor();
            yaml.LoadFromString(data);
            var jobs = yaml.jobs;
            var dtJobs = new DataTable("jobs" );
            dtJobs.Columns.Add(new DataColumn("name", typeof(string)));
            dtJobs.Columns.Add(new DataColumn("condition", typeof(string)));
            dtJobs.Columns.Add(new DataColumn("pool", typeof(string)));

            var dtSteps = new DataTable("steps");
            dtSteps.Columns.Add("jobName", typeof(string));
            dtSteps.Columns.Add(new DataColumn("displayName", typeof(string)));
            dtSteps.Columns.Add(new DataColumn("name", typeof(string)));
            dtSteps.Columns.Add(new DataColumn("value", typeof(string)));

            var dtDepends = new DataTable("jobDepends");
            dtDepends.Columns.Add("jobName", typeof(string));
            dtDepends.Columns.Add("jobNameDepends", typeof(string));

            int idJob = 0;
            foreach (var job in jobs)
            {
                idJob++;
                if (string.IsNullOrEmpty(job.Name))
                    job.Name = $"NewJob{idJob}";
                dtJobs.Rows.Add(job.Name, job.condition, job.pool.ToString());
                
                foreach(var step in job.Steps)
                {
                    dtSteps.Rows.Add(job.Name, step.DisplayName, step.Name, step.Value);    
                }
                foreach(var name in job.DependsOn)
                {
                    dtDepends.Rows.Add(job.Name, name);
                }

            }
            var id = receiveData.AddNewTable(dtJobs);
            receiveData.Metadata.AddTable(dtJobs, id);

            id = receiveData.AddNewTable(dtSteps);
            receiveData.Metadata.AddTable(dtSteps, id);

            id = receiveData.AddNewTable(dtDepends);
            receiveData.Metadata.AddTable(dtDepends, id);

            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
