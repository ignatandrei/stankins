using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Data;
using System.Threading.Tasks;
using dia= System.Diagnostics;
namespace Stankins.Process
{
    public class ReceiverProcess : BaseObject,  IReceive
    {
        public ReceiverProcess(string fileName, string arguments):this(new CtorDictionary()
        {
            {nameof(fileName), fileName },
            {nameof(arguments),arguments }
        })
        {
           
        }
        public ReceiverProcess(CtorDictionary dict):base(dict)
        {
            this.FileName = GetMyDataOrThrow<string>(nameof(FileName));
            this.Arguments= GetMyDataOrDefault<string>(nameof(Arguments),"");
        }

        ProcessIntercept process ;
        DataTable output = new DataTable();
        DataTable error = new DataTable();
        public string FileName { get; }
        public string Arguments { get; }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            
            
            output = new DataTable();
            output.Columns.Add(new DataColumn("message", typeof(string)));

            error = new DataTable();
            error.Columns.Add(new DataColumn("message", typeof(string)));

            var ret = new DataToSentTable();
            var id=  ret.AddNewTable(output);
            ret.Metadata.AddTable(output, id);
            id= ret.AddNewTable(error);
            ret.Metadata.AddTable(error, id);
            process = new ProcessIntercept(FileName, Arguments);
            process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(Process_OutputDataReceived);
            process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(Process_ErrorDataReceived);
            process.Exited += new System.EventHandler(Process_Exited);
            process.StartProcessAndWait();
            return await Task.FromResult(ret);
        }
        void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("process exited with code {0}\n", process.ExitCode.ToString()));
        }

        void Process_ErrorDataReceived(object sender, dia.DataReceivedEventArgs e)
        {
            error.Rows.Add(new[] { e.Data });
        }

        void Process_OutputDataReceived(object sender, dia.DataReceivedEventArgs e)
        {
            output.Rows.Add(new[] { e.Data });
        }
        public override Task<IMetadata> TryLoadMetadata()
        {
            //TODO: it is know - output and error tabls
            throw new NotImplementedException();
        }
    }
}
