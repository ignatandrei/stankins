using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StankinsCommon
{
    public class SendDataWeb
    {
        public async Task<string> PostJSON(string url, string data)
        {
            using(var wc=new WebClient())
            {
                
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var res= await wc.UploadStringTaskAsync(url,"POST" ,data);
                return res;
                
            }
        }
        
    }
}
