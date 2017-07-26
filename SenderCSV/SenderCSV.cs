using StankinsInterfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenderCSV
{
    /// <summary>
    /// TODO:
    /// 1. CSV separator : ,  or any other
    /// 2. File.Write or File.Append
    /// 3. For null what to write?
    /// 4. For big data - to write in chunks  - size
    /// </summary>
    public class SenderToCSV : ISend
    {

        
        public IRow[] valuesToBeSent { private get; set; }
        public string CSVFileName{ get; set; }
        public SenderToCSV(string fileName)
        {
            this.CSVFileName = fileName;
        }
        
        
        public async Task Send()
        {
            

            if(valuesToBeSent?.Length == 0)
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

            for(var i = 0; i < nrValues; i++)
            {
                var row = valuesToBeSent[i];

                sb.AppendLine(
                    string.Join(",",
                        row.Values.Select(it => it.Value?.ToString()).ToArray())
                    );
            }
            
            File.WriteAllText(CSVFileName, sb.ToString());

        }
    }
}
