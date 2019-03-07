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
            Name = nameof(ReceiverPing);
        }
        public ReceiverPing(string nameSite):this(new CtorDictionary()
        {
            {nameof(nameSite),nameSite }
        })
        {
            
        }

        public string NameSite { get; private set; }
        
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
                receiveData = new DataToSentTable();
            var sw = Stopwatch.StartNew();
            DataTable results = CreateTable();
            var pingSender = new Ping();
            var StartedDate = DateTime.UtcNow;
            try
            {
                var reply = pingSender.Send(NameSite);
                
                results.Rows.Add("ping", "", NameSite,true, (int)reply.Status, reply.RoundtripTime.ToString(), reply.RoundtripTime.ToString(),null,StartedDate);
            }
            catch(Exception ex)
            {
                results.Rows.Add("ping", "", NameSite,false, null, sw.ElapsedMilliseconds, null, ex.Message,StartedDate);
            }
            receiveData.AddNewTable(results);
            receiveData.Metadata.AddTable(results, receiveData.Metadata.Tables.Count);

            
            return await Task.FromResult(receiveData) ;
        }

        

    }
}
