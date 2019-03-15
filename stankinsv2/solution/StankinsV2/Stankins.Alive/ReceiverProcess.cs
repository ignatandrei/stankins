using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using dia=System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Stankins.Alive
{
    public class ReceiverProcessAlive : AliveStatus, IReceive
    {
        public string FileName { get; }
        public string Arguments { get; }
        public ReceiverProcessAlive(string fileName, string arguments) : this(new CtorDictionary()
        {
            {nameof(fileName), fileName },
            {nameof(arguments),arguments }
        })
        {

        }
        public ReceiverProcessAlive(CtorDictionary dict) : base(dict)
        {
            this.FileName = GetMyDataOrThrow<string>(nameof(FileName));
            this.Arguments = GetMyDataOrDefault<string>(nameof(Arguments), "");
            this.Name = nameof(ReceiverProcessAlive);
        }
        DataTable results;
        Stopwatch sw;
        DateTime StartedDate;
        ProcessIntercept process;
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
                receiveData = new DataToSentTable();
            results = CreateTable(receiveData);
            sw = Stopwatch.StartNew();
            StartedDate = DateTime.UtcNow;
            try
            {

                process = new ProcessIntercept(FileName, Arguments);
                process.OutputDataReceived += new dia.DataReceivedEventHandler(Process_OutputDataReceived);
                process.ErrorDataReceived += new dia.DataReceivedEventHandler(Process_ErrorDataReceived);
                process.Exited += new System.EventHandler(Process_Exited);
                process.StartProcessAndWait();

            }
            catch (Exception ex)
            {
                results.Rows.Add("process",Arguments, FileName + Arguments , false, ex.Message, sw.ElapsedMilliseconds, "", ex.Message, StartedDate);
            }

          

            return await Task.FromResult(receiveData) ;
        }
        void Process_ErrorDataReceived(object sender, dia.DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e?.Data))
                return;

            results.Rows.Add("process", Arguments, FileName + Arguments, false,e.Data,sw.ElapsedMilliseconds, e.Data, null, StartedDate);
        }

        void Process_OutputDataReceived(object sender, dia.DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e?.Data))
                return;

            results.Rows.Add("process", Arguments, FileName + Arguments, true, e.Data, sw.ElapsedMilliseconds, e.Data, null, StartedDate);
        }
        void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("process exited with code {0}\n", process.ExitCode.ToString()));
        }
    }
}
