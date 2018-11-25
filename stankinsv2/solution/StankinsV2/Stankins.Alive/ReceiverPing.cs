using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Stankins.Alive
{
    public class ReceiverPing : AliveStatus, IReceive
    {
        public ReceiverPing(CtorDictionary dict) : base(dict)
        {
            NameSite =GetMyDataOrThrow<string>(nameof(NameSite));
        }
        public ReceiverPing(string nameSite):this(new CtorDictionary()
        {
            {nameof(nameSite),nameSite }
        })
        {
            
        }

        public string NameSite { get; private set; }
        DataTable results;
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
                receiveData = new DataToSentTable();

            results = CreateTable();
            var pingSender = new Ping();
            var reply = pingSender.Send(NameSite);
            var status = reply.Status;
            results.Rows.Add("ping","",NameSite,(int)reply.Status,reply.RoundtripTime.ToString() );
            receiveData.AddNewTable(results);
            receiveData.Metadata.AddTable(results, receiveData.Metadata.Tables.Count);

            //var p = new ProcessIntercept("ping.exe",NameSite);
            //p.OutputDataReceived += P_OutputDataReceived;

            //receiveData.AddNewTable(results);
            //results.TableName = "ping.exe " + NameSite; 
            //p.StartProcessAndWait();
            return receiveData;
        }

        
        //private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    var str = e?.Data?.Trim();
        //    if ((str?.Length ?? 0) == 0)
        //        return;

        //}
    }
}
