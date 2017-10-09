using SenderToFile;
using MediaTransform;
namespace SenderHTML
{
    public class Sender_Text: SenderMediaToFile
    {
        public Sender_Text(string outputFileName, string text) :base(outputFileName,new MediaTransformStringToText(text))
        {
            this.Name = $"send {text} to {outputFileName}";
        }
    }
}
