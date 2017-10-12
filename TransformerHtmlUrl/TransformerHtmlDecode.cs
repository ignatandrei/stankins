using StankinsInterfaces;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TransformerHtmlUrl
{
    public class TransformerHtmlDecode : ITransform
    {
        public IRow[] valuesRead { get ; set ; }
        public IRow[] valuesTransformed { get ; set ; }
        public string Name { get ; set ; }
        public TransformerHtmlDecode()
        {
            Name = "decode html";
        }
        public async Task Run()
        {
            valuesTransformed = valuesRead;
            foreach(var vt in valuesTransformed)
            {
                var keys = vt.Values.Keys.ToArray();
                foreach (var key in keys)
                {
                    vt.Values[key] = WebUtility.HtmlDecode(vt.Values[key]?.ToString());
                }
            }
        }
    }
}
