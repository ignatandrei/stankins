using StankinsCommon;

namespace Stankins.Razor
{
    public class SenderToRazorWithContents : SenderToRazor
    {
        public SenderToRazorWithContents(string InputTemplate) : this(new CtorDictionary() {
                { nameof(InputTemplate), InputTemplate}

            }
        )
        {
           
        }
        public SenderToRazorWithContents(CtorDictionary dataNeeded) : base(dataNeeded)
        {

          

            this.Name = nameof(SenderToRazorWithContents);



        }
        public override string DefaultText()
        {
            return "please construct with a InputTemplate string that is relevant";
        }
    }
}