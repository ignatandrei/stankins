using System.IO;
using StankinsCommon;

namespace Stankins.Razor
{
    public class SenderToRazorFromFile: SenderToRazor
    {

        public SenderToRazorFromFile(string InputTemplate) : this(new CtorDictionary()
        {
            { nameof(InputTemplate), File.ReadAllText(InputTemplate)}

        })
        {
            this.Name = nameof(SenderToRazorFromFile);
            
        }
        public SenderToRazorFromFile(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderToRazorFromFile);
            
        }

        public override string DefaultText()
        {
            return "not a text";
        }
    }
}