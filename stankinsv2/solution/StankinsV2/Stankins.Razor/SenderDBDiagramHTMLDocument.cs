using Stankins.Interfaces;
using StankinsCommon;

namespace Stankins.Razor
{
    public class SenderDBDiagramHTMLDocument : SenderToRazor, ISenderToOutput
    {
        public SenderDBDiagramHTMLDocument(string inputTemplate=null) : 
            this(new CtorDictionary() {
                { nameof(inputTemplate), inputTemplate}

            }
        )
        
        {
            this.Name = nameof(SenderDBDiagramHTMLDocument);
        }

        public SenderDBDiagramHTMLDocument(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderDBDiagramHTMLDocument);
        }

        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderDBDiagramHTMLDocument)}.cshtml");
        }
    }
}