using RazorLight;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.Razor
{
    public class SenderDBDiagramToDot : BaseObject, ISenderToOutput
    {

        public string InputContents { get; set; }
        public KeyValuePair<string, string>[] OutputContents { get; set; }

        public SenderDBDiagramToDot(string inputContents) : this(new CtorDictionary() {
            { nameof(inputContents), inputContents}
            
            }
          )
        {

        }
        public SenderDBDiagramToDot(CtorDictionary dataNeeded) : base(dataNeeded)
        {
           
            this.InputContents = base.GetMyDataOrDefault<string>(nameof(InputContents),null);
            if (string.IsNullOrWhiteSpace(this.InputContents))
                this.InputContents = File.ReadAllText("erdotviz.cshtml");



        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var engine = new RazorLightEngineBuilder()
           .UseMemoryCachingProvider()
           .Build();
            var key = Guid.NewGuid().ToString();
            var found= await engine.CompileRenderAsync<IDataToSent>(key,InputContents,receiveData);
            
            
            OutputContents = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(key, found) };



            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
