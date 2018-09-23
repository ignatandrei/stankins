using Stankins.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.File
{
    public class ReceiverStreamingText : IStreamingReceive<string>
    {
        public ReceiverStreamingText(string text, char separator)
        {
            Text = text;
            Separator = separator;
        }

        public string Text { get; }
        public Encoding Encoding { get; }
        public char Separator { get; }

        public async Task<bool> Initialize()
        {
            return true;
        }


        public async Task<string[]> StreamData()
        {
            var splitLines = Text.Split(Separator);
            return splitLines;
        }
    }
}