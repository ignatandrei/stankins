using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.FileOps
{

    public class StreamToStringCSV : IStreaming<String>
    {
        public async Task<bool> Initialize()
        {
            return await Task.FromResult(true);
        }

        public IEnumerable<string> StreamTo(IDataToSent dataToSent)
        {
            foreach (var dt in dataToSent.DataToBeSentFurther)
            {
                var sb = new StringBuilder();
                var cols = new List<string>();
                foreach (DataColumn dc in dt.Value.Columns)
                {
                   
                   cols.Add(dc.ColumnName);

                }
                sb.AppendLine(string.Join(",", cols.ToArray()));
                foreach (DataRow dr in dt.Value.Rows)
                {
                    sb.AppendLine(string.Join(",", dr.ItemArray));
                }
                yield return sb.ToString();

            }
        }
    }
}
