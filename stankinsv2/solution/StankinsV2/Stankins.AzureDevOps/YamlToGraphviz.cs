using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.AzureDevOps
{
    public class YamlToGraphviz : BaseObject, ISender, IStreaming<string>
    {

        public YamlToGraphviz() : base(null)
        {
        }
        StringBuilder sb;
        public async Task<bool> Initialize()
        {
            sb = new StringBuilder();
            sb.AppendLine("digraph G{");
            return true;
        }

        public IEnumerable<string> StreamTo(IDataToSent dataToSent)
        {
            var result = new StringBuilder();
            var nodes = new StringBuilder();
            var jobs = dataToSent.FindAfterName("jobs");
            var tasks = dataToSent.FindAfterName("steps");
            int iJob = 0;
            foreach(DataRow job in jobs.Value.Rows)
            {
                iJob++;
                var jobName = job["name"].ToString();
                result.AppendLine($@"subgraph cluster_{iJob} {{
                        label = ""job {jobName}"";");
                
                
                nodes.AppendLine($"NodeJob{iJob} [label=\"{jobName}\"];");
                result.Append($"NodeJob{iJob} ");
                int iTask = 0;
                foreach ( DataRow step  in tasks.Value.Rows)
                {
                    iTask++;
                    if (step["jobName"].ToString() != jobName)
                        continue;
                    var taskName = step["displayName"].ToString();
                    nodes.AppendLine($"NodeJob{iJob}_{iTask} [label=\"{taskName}\"];");
                    result.Append($" -> NodeJob{iJob}_{iTask}");
                }

                result.AppendLine("}");

            }
            var res = result.ToString()+ Environment.NewLine + nodes.ToString();
            yield return res;
        }
        public string Result() =>  sb?.ToString();

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (sb == null)
                await Initialize();


            var arr = this.StreamTo(receiveData);
            sb.AppendLine(arr.First());
            sb.AppendLine("}");

            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
