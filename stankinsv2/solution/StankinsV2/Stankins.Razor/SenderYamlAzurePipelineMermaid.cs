using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stankins.Razor;

namespace Stankins.AzureDevOps
{
    public class SenderYamlAzurePipelineMermaid :  SenderToRazor, ISenderToOutput
    { 
        public SenderYamlAzurePipelineMermaid(CtorDictionary dict) : base(dict)
        {
            this.Name = nameof(SenderYamlAzurePipelineToDot);
        }
        public SenderYamlAzurePipelineMermaid(string inputTemplate=null) : this(new CtorDictionary() {
                { nameof(InputTemplate), inputTemplate}

            })
        {
        } 
        
        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderYamlAzurePipelineMermaid)}.cshtml");
        }
             
        //public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        //{
        //    if (sb == null)
        //        await Initialize();


        //    var arr = this.StreamTo(receiveData);
        //    sb.AppendLine(arr.First());
        //    sb.AppendLine("}");

        //    base.CreateOutputIfNotExists(receiveData);
        //    var key = Guid.NewGuid().ToString();
        //    base.OutputString.Rows.Add(null, key, sb.ToString());
        //    return await Task.FromResult(receiveData) ;
        //}

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
