using System.Text;

namespace ReceiverCSV
{
    public class ReceiverCSVFileInt: ReceiverCSVFile<int>
    {
        public ReceiverCSVFileInt(string fileName, Encoding encoding): base(fileName,encoding)
        {
        }

    }
}
