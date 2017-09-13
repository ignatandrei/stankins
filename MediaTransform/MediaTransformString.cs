using StankinsInterfaces;
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
    public abstract class MediaTransformString : IFilterTransformToString
    {
        public IRow[] valuesToBeSent { set; protected get; }

        public string Result { get; protected set; }

        public abstract  Task Run();
        public string Name { get; set; }
    }
}
