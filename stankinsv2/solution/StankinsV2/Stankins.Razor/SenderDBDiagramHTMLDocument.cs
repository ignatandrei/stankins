using Stankins.Interfaces;
using StankinsCommon;

namespace Stankins.Razor
{
    public class SenderDBDiagramHTMLDocument : SenderDBDiagram, ISenderToOutput
    {
        public SenderDBDiagramHTMLDocument(string inputContents) : base(inputContents)
        {
        }

        public SenderDBDiagramHTMLDocument(CtorDictionary dataNeeded) : base(dataNeeded)
        {
        }

        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderDBDiagramHTMLDocument)}.cshtml");
        }
    }
}