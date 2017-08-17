using MediaTransform;

namespace SenderToFile
{
    public class Sender_JSON : SenderMediaToFile
    {
        public Sender_JSON(string fileName):base(new MediaTransformJSON(), fileName)
        {

        }
       
    }
}
