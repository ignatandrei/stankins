using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stankins.Razor
{

    /// <summary>
    /// Usage in console:
    /// execute -o ReceiveRestFromFile -a e:\ignatandrei\stankins\stankinsv2\solution\StankinsV2\StankinsTestXUnit\Assets\JSON\jsonAlphabetMoreTables.txt -o SenderToTypeScript -a "" 
    /// </summary>
    public class SenderToTypeScript: SenderToRazor, ISenderToOutput
    {
        public SenderToTypeScript(string inputTemplate=null) : this(
            new CtorDictionary()
            {
                {nameof(inputTemplate),inputTemplate }
            })            
        {
            
        }

        public SenderToTypeScript(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderToTypeScript);
            
        }

        public override string DefaultText()
        {
            return  base.ReadFile($"{nameof(SenderToTypeScript)}.cshtml");
        }
    }
    
}
