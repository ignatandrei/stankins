using System.Threading.Tasks;
using StankinsCommon;

namespace Stankins.Rest
{
    public class ReceiverRestFromText : ReceiveRest
    {
        private readonly string text;

        public ReceiverRestFromText(string text):this(new CtorDictionary()
        {
            {nameof(text),text}
        })
        {
            
        }
        public ReceiverRestFromText(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.text = GetMyDataOrThrow<string>(nameof(text));
        }

        public override Task<string> GetData()
        {
            return Task.FromResult(text);
        }
    }
}
