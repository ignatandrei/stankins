using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stankins.Razor;

namespace Stankins.AzureDevOps
{
    public class SenderYamlAzurePipelineToDot :  SenderToRazor, ISenderToOutput
    {
        public SenderYamlAzurePipelineToDot(CtorDictionary dict) : base(dict)
        {
            this.Name = nameof(SenderYamlAzurePipelineToDot);
        }
        public SenderYamlAzurePipelineToDot(string inputTemplate=null) : this(new CtorDictionary() {
                { nameof(InputTemplate), inputTemplate}

            })
        {
        }
        //StringBuilder sb;

       
        //public async Task<bool> Initialize()
        //{
        //    sb = new StringBuilder();
        //    sb.AppendLine("digraph G{");
        //    return await Task.FromResult(true);
        //}

        //public   IEnumerable<string> StreamTo(IDataToSent dataToSent)
        //{
        //    var result = new StringBuilder();
        //    var nodes = new StringBuilder();
        //    var jobs = dataToSent.FindAfterName("jobs");
        //    var tasks = dataToSent.FindAfterName("steps");
        //    int iJob = 0;
        //    var dictJobs = new Dictionary<string, (string JobNode, string lastTaskNode)>();
        //    foreach(DataRow job in jobs.Value.Rows)
        //    {
        //        iJob++;
        //        var jobName = job["name"].ToString();
        //        result.AppendLine($@"subgraph cluster_{iJob} {{
        //                label = ""job {jobName}"";");
        //        var lastNode = $"NodeJob{iJob}";
        //        nodes.AppendLine($"NodeJob{iJob} [label=\"{jobName}\" shape=folder color=lightblue] ;");
        //        result.Append($"NodeJob{iJob} ");
        //        int iTask = 0;
        //        foreach ( DataRow step  in tasks.Value.Rows)
        //        {
        //            iTask++;
        //            if (step["jobName"].ToString() != jobName)
        //                continue;
        //            var taskName = step["displayName"].ToString();
        //            if(string.IsNullOrWhiteSpace(taskName))
        //                taskName= step["name"].ToString();

        //            nodes.AppendLine($"NodeJob{iJob}_{iTask} [label=\"{taskName}\"];");
        //            result.Append($" -> NodeJob{iJob}_{iTask}");
        //            lastNode = $"NodeJob{iJob}_{iTask}";
        //        }
        //        dictJobs.Add(jobName, ($"NodeJob{iJob}", lastNode));


        //        result.AppendLine("}");

        //    }
        //    var depends = dataToSent.FindAfterName("jobDepends");
        //    var dependsString = new StringBuilder();
        //    foreach (DataRow job in depends.Value.Rows)
        //    {
        //        var jobName= job["jobName"].ToString();
        //        var jobDepends = job["jobNameDepends"].ToString();
        //        var node= dictJobs[jobName];
        //        var nodeDepends = dictJobs[jobDepends];
        //        dependsString.AppendLine($"{nodeDepends.lastTaskNode}-> {node.JobNode} ");
        //    }
        //        var res = result.ToString()+ Environment.NewLine + nodes.ToString() + Environment.NewLine + dependsString.ToString();
        //    yield return res;
        //}
        //public string Result() =>  sb?.ToString();

        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderYamlAzurePipelineToDot)}.cshtml");
        }

        //public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        //{
        //    if (sb == null)
        //        await Initialize();


        //    var arr = this.StreamTo(receiveData);
        //    sb.AppendLine(arr.First());
        //    sb.AppendLine("}");

        //    base.CreateOutputIfNotExists(receiveData);
        //    var key = Guid.NewGuid().ToString();
        //    base.OutputString.Rows.Add(null, key, sb.ToString());
        //    return await Task.FromResult(receiveData) ;
        //}

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
