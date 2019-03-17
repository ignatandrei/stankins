using Stankins.Interfaces;
using StankinsCommon;
using System.IO;
using System.Text;
using StankinsObjects;

namespace Stankins.Razor
{
    
    public class SenderDBDiagramToDot : SenderToRazor, ISenderToOutput
    {

        public SenderDBDiagramToDot(string inputTemplate=null) : this(new CtorDictionary() {
                { nameof(InputTemplate), inputTemplate}

            })
        {
            this.Name = nameof(SenderDBDiagramToDot);
        }
        public SenderDBDiagramToDot(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderDBDiagramToDot);
        }

        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderDBDiagramToDot)}.cshtml");
        }
    }
}
