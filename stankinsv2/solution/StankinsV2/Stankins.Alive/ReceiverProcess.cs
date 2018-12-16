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
        }
        DataTable results;
        Stopwatch sw;
        DateTime StartedDate;
        ProcessIntercept process;
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
                receiveData = new DataToSentTable();
            results = CreateTable();
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
                results.Rows.Add(FileName, Arguments, "", false, ex.Message, sw.ElapsedMilliseconds, "", ex.Message, StartedDate);
            }

            receiveData.AddNewTable(results);
            receiveData.Metadata.AddTable(results, receiveData.Metadata.Tables.Count);

            return receiveData;
        }
        //var m = new DataTable();
        //m.Columns.Add("Process",typeof(string));
        //    m.Columns.Add("Arguments", typeof(string));
        //    m.Columns.Add("To", typeof(string));            
        //    m.Columns.Add("IsSuccess", typeof(bool));
        //    m.Columns.Add("Result", typeof(string));
        //    m.Columns.Add("Duration", typeof(long));
        //    m.Columns.Add("DetailedResult", typeof(string));
        //    m.Columns.Add("Exception", typeof(string));
        //    m.Columns.Add("StartedDate", typeof(DateTime));
        void Process_ErrorDataReceived(object sender, dia.DataReceivedEventArgs e)
        {
            results.Rows.Add(FileName,Arguments,"",true,e.Data,sw.ElapsedMilliseconds,"",null, StartedDate);
        }

        void Process_OutputDataReceived(object sender, dia.DataReceivedEventArgs e)
        {
            results.Rows.Add(FileName, Arguments, "", false, e.Data, sw.ElapsedMilliseconds, "", null, StartedDate);
        }
        void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("process exited with code {0}\n", process.ExitCode.ToString()));
        }
    }
}
