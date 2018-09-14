using MediaTransform;

namespace SenderToFile
{
    public class Sender_Binary : SenderMediaToFile
    {
        public Sender_Binary(string fileName):base(fileName, new MediaTransformBSON())
        {

        }
       
    }
}
