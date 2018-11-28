using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.Alive
{
    public class ReceiverWeb : AliveStatus, IReceive
    {
        public ReceiverWeb(CtorDictionary dict) : base(dict)
        {
            URL = GetMyDataOrThrow<string>(nameof(URL));
            Method = GetMyDataOrDefault<string>(nameof(Method), "GET");
            if (string.IsNullOrWhiteSpace(Method))
                Method = "GET";
        }
        public ReceiverWeb(string url) : this(url, null)
        {

        }
        public ReceiverWeb(string url,string method) : this(new CtorDictionary()
        {
            {nameof(url),url},
            {nameof(method),method }
        })
        {
        }

        public string URL { get; private set; }
        DataTable results;
        private readonly string Method;

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
                receiveData = new DataToSentTable();
            results = CreateTable();

            var ws = WebRequest.Create(URL) as HttpWebRequest;
            ws.Method = Method;
            try
            {
                using (var resp = await ws.GetResponseAsync())
                {
                    var r = resp as HttpWebResponse;
                    var sc = (int)r.StatusCode;
                    var text = "";
                    if ((sc >= 200) && (sc <= 299))
                    {
                        using (var rs = r.GetResponseStream())
                        {
                            using (var sr = new StreamReader(rs))
                            {
                                text = await sr.ReadToEndAsync();
                            }
                        }
                    }
                    results.Rows.Add("webrequest", Method, URL, sc, text,null);



                }
            }
            catch(Exception ex)
            {
                results.Rows.Add("webrequest", Method, URL, null, null, ex.Message);
            }
            receiveData.AddNewTable(results);
            receiveData.Metadata.AddTable(results, receiveData.Metadata.Tables.Count);

            return receiveData;
        }
    }
}
