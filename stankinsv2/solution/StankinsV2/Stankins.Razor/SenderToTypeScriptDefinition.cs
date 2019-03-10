using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stankins.Razor
{
    public class SenderToTypeScriptDefinition: SenderToRazor, ISenderToOutput
    {
        public SenderToTypeScriptDefinition(string inputTemplate=null) : this(
            new CtorDictionary()
            {
                {nameof(inputTemplate),inputTemplate }
            })            
        {
            
        }

        public SenderToTypeScriptDefinition(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderTrelloTo);
            
        }

        public override string DefaultText()
        {
            return  base.ReadFile($"{nameof(SenderToTypeScriptDefinition)}.cshtml");
        }
    }
    
}
