using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stankins.Razor
{
    public class SenderSolutionToDot : SenderToRazor, ISenderToOutput
    {
        public SenderSolutionToDot(string inputContents = null) : base(inputContents)
        {
            this.Name = nameof(SenderSolutionToDot);
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
