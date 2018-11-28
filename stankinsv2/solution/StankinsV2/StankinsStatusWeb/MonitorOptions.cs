using Stankins.Alive;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class MonitorOptions
    {
        public WebAdress[] WebAdresses { get; set; }
        public PingAddress[] PingAddresses { get; set; } 

    }
    public class WebAdress
    {
        public string URL { get; set; }
        public async Task<DataTable> Execute()
        {
            var r = new ReceiverWeb(URL);
            var ret = await r.TransformData(null);
            return ret.DataToBeSentFurther.Values.First();
        }
    }

    public class PingAddress
    {
        public string NameSite { get; set; }
        public async Task<DataTable> Execute()
        {
            var r = new ReceiverPing(NameSite);
            var ret = await r.TransformData(null);
            return ret.DataToBeSentFurther.Values.First();
        }
    }
}
