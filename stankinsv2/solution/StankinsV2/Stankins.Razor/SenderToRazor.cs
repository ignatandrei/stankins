using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RazorLight;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.Razor
{
    public abstract class SenderToRazor : BaseObjectSender, ISenderToOutput
    {

        public SenderToRazor(string inputContents) : this(new CtorDictionary() {
                { nameof(inputContents), inputContents}

            }
        )
        {

        }
        public SenderToRazor(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            this.InputTemplate = base.GetMyDataOrDefault<string>(nameof(InputTemplate), null);
            if (string.IsNullOrWhiteSpace(this.InputTemplate))
                this.InputTemplate =DefaultText();

            this.Name = nameof(SenderToRazor);



        }
        public abstract string DefaultText();
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var engine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();
            var key = Guid.NewGuid().ToString();
            var found = await engine.CompileRenderAsync<IDataToSent>(key, InputTemplate, receiveData);

            base.CreateOutputIfNotExists(receiveData);

            base.OutputString.Rows.Add(null, key, found);
            



            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }

    }
}