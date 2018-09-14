using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransform
{

    /// <summary>
    /// TODO: add media transform json + xml
    /// +html + pdf + word +excel
    /// TODO: Remove projects that have sendercsv, sender html, sender json
    /// sender xml and replace with implementation for
    /// file with Media Transform
    /// </summary>
    public class MediaTransformCSV : MediaTransformString
    {
        
        public override async Task Run()
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
            await Task.CompletedTask;

        }
    }
}
