using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects 
{
    public class TransformTrim : BaseObject, ITransformer
    {
        public TransformTrim() : this(null)
        {
            
        }
        public TransformTrim(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(TransformTrim);
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            foreach (var dt in receiveData.DataToBeSentFurther)
            {
                var colString = new List<string>();
                foreach(DataColumn dc in dt.Value.Columns)
                {
                    if(dc.DataType == typeof(string))
                    {
                        colString.Add(dc.ColumnName);
                    }
                }
                if (colString.Count == 0)
                    continue;
                foreach(DataRow dr in dt.Value.Rows)
                {
                    foreach(var col in colString)
                    {
                        dr[col] = dr[col]?.ToString()?.Trim();
                    }
                }
                

            }
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
