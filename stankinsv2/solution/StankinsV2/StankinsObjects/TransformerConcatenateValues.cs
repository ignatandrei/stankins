using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;

namespace StankinsObjects
{
    public class TransformerConcatenateOutputString:BaseObject,ITransformer
    {
        private readonly string NewTotalNameOutput;

        public TransformerConcatenateOutputString(CtorDictionary dataNeeded):base(dataNeeded)
        {
       
            this.Name = nameof(TransformerConcatenateOutputString);
            this.NewTotalNameOutput = base.GetMyDataOrDefault<string>(nameof(NewTotalNameOutput), "total");
        }
        public TransformerConcatenateOutputString(string newTotalNameOutput) :this(new CtorDictionary()
        {
            {nameof(newTotalNameOutput),newTotalNameOutput }
        })
        {
            
        }
        
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        { 
            StringBuilder res = new StringBuilder();
            var dt = receiveData.FindAfterName("OutputString").Value;
            foreach (DataRow valueRow in dt.Rows)
            {
                res.Append(valueRow["Contents"]?.ToString());

            }
            dt.Clear();
            dt.Rows.Add(null, NewTotalNameOutput, res.ToString());
            return await Task.FromResult(receiveData);
        }

       

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
