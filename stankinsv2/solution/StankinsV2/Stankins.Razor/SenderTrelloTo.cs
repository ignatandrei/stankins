using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stankins.Razor
{
    public class SenderTrelloTo: SenderToRazor, ISenderToOutput
    {
        public SenderTrelloTo(string inputTemplate=null) : this(
            new CtorDictionary()
            {
                {nameof(inputTemplate),inputTemplate }
            })            
        {
            
        }

        public SenderTrelloTo(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderTrelloTo);
        }

        public override string DefaultText()
        {
            return "please choose a trello file ";
        }
    }
}