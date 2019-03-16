using Stankins.Interfaces;
using Stankins.Jenkins;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Stankins.Alive
{
    public class ReceiverJenkins : AliveStatus, IReceive
    {
        private readonly string url;

        public ReceiverJenkins(string url) : this(new CtorDictionary()
        {
            {nameof(url),url }

            })
        {
            this.url = url;
        }
        public ReceiverJenkins(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            url = GetMyDataOrThrow<string>(nameof(url));
            Name = nameof(ReceiverJenkins);
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData = new DataToSentTable();
            }

            JenkinsJson res = new JenkinsJson(url);
            System.Data.DataTable results = CreateTable(receiveData);
            Stopwatch sw = Stopwatch.StartNew();
            DateTime StartedDate = DateTime.UtcNow;
            try
            {
                IDataToSent data = await res.TransformData(receiveData);
                results.Rows.Add("jenkinssite", "/api/json", url, true, "", sw.ElapsedMilliseconds, "", null, StartedDate);
                return data;
            }
            catch (Exception ex)
            {
                results.Rows.Add("jenkinssite", "", url, false, null, sw.ElapsedMilliseconds, null, ex.Message, StartedDate);

            }
            return receiveData;
        }
    }
}
