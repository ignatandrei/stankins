using Stankins.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.FileOps
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
            return await Task.FromResult(true);
        }


        public async Task<string[]> StreamData()
        {
            var splitLines = Text.Split(Separator);
            return await Task.FromResult(splitLines);
        }
    }
}