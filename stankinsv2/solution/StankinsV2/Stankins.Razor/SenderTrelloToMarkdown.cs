using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stankins.Razor
{
    public class SenderTrelloToMarkdown: SenderToRazor, ISenderToOutput
    {
        public SenderTrelloToMarkdown(string inputContents=null) : base(inputContents)
        {
            this.Name = nameof(SenderDBDiagramHTMLDocument);
        }

        public SenderTrelloToMarkdown(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderDBDiagramHTMLDocument);
        }

        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderTrelloToMarkdown)}.cshtml");
        }
    }
}