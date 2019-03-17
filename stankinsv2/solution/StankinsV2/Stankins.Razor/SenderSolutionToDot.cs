using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stankins.Razor
{
    public class SenderSolutionToDot : SenderToRazor, ISenderToOutput
    {
        public SenderSolutionToDot(string inputTemplate = null) :this(new CtorDictionary() {
                { nameof(InputTemplate), inputTemplate}

            })
        {
           
        }

        public SenderSolutionToDot(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderSolutionToDot);
        }

        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderSolutionToDot)}.cshtml");
        }
    }
}
