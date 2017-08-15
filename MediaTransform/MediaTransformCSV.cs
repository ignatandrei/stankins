using StankinsInterfaces;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransform
{
    public class MediaTransformCSV : IFilterTransformer
    {
        public IRow[] valuesToBeSent { private get; set; }
        public string Result { get; set; }
        public async Task Run()
        {
            if (valuesToBeSent?.Length == 0)
            {
                //LOG: there are no data 
                return;
                
            }
            var sb = new StringBuilder();
            var FieldNameToMarks = valuesToBeSent[0]
                .Values
                .Select(it => it.Key).ToArray();

            sb.AppendLine(string.Join(",", FieldNameToMarks));
            var nrValues = valuesToBeSent.LongCount();

            for (var i = 0; i < nrValues; i++)
            {
                var row = valuesToBeSent[i];

                sb.AppendLine(
                    string.Join(",",
                        row.Values.Select(it => it.Value?.ToString()).ToArray())
                    );
            }
            Result = sb.ToString();

            
        }
    }
}
