using System.IO;
using StankinsCommon;

namespace Stankins.Razor
{
    public class SenderToRazorFromFile: SenderToRazor
    {

        public SenderToRazorFromFile(string nameFile) : this(new CtorDictionary()
        {
            { "inputContents", File.ReadAllText(nameFile)}

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