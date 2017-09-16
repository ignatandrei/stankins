using System.Threading.Tasks;

namespace MediaTransform
{
    public class MediaTransformStringToText:MediaTransformString
    {
        public MediaTransformStringToText(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
        public override async Task Run()
        {
            Result = Text;
            await Task.CompletedTask;
        }
    }
}
