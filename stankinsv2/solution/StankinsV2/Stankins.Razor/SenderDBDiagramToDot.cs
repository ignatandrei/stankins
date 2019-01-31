using Stankins.Interfaces;
using StankinsCommon;
using System.IO;
using System.Text;
using StankinsObjects;

namespace Stankins.Razor
{
    
    public class SenderDBDiagramToDot : SenderDBDiagram, ISenderToOutput
    {

        public SenderDBDiagramToDot(string inputContents=null) : base(inputContents)
        {

        }
        public SenderDBDiagramToDot(CtorDictionary dataNeeded) : base(dataNeeded)
        {
           
        }

        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderDBDiagramToDot)}.cshtml");
        }
    }
}
