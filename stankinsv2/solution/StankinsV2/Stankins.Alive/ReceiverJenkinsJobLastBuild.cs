using Stankins.Interfaces;
using Stankins.Jenkins;
using Stankins.Rest;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Stankins.Alive
{
    public class ReceiverJenkinsJobLastBuild : AliveStatus, IReceive
    {
        private readonly string url;
        private readonly string jobName;

        public ReceiverJenkinsJobLastBuild(string url, string jobName) : this(new CtorDictionary()
        {
            {nameof(url),url },
              {nameof(jobName),jobName }

            })
        {
          
        }
        public ReceiverJenkinsJobLastBuild(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            url = GetMyDataOrThrow<string>(nameof(url));
            jobName = GetMyDataOrThrow<string>(nameof(jobName));
            Name = nameof(ReceiverJenkins);
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData = new DataToSentTable();
            }
            var jobUrl= url+"/job/"+jobName+"/lastBuild/api/json";
            var res = new ReceiveRestFromFile(jobUrl);
            var results = CreateTable(receiveData);
            var sw = Stopwatch.StartNew();
            DateTime StartedDate = DateTime.UtcNow;
            try
            {
                var data = await res.TransformData(receiveData);
                results.Rows.Add("jenkinsjob", "/job/"+jobName+"/lastBuild/api/json", url, true, "", sw.ElapsedMilliseconds, "", null, StartedDate);
                return data;
            }
            catch (Exception ex)
            {
                results.Rows.Add("jenkinsjob", "", url, false, null, sw.ElapsedMilliseconds, null, ex.Message, StartedDate);

            }
            return receiveData;
        }
    }
}
