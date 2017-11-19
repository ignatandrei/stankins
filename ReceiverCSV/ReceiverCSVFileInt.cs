using System.Text;

namespace ReceiverCSV
{
    /// <summary>
    /// CSV receiver for last value of int data type.
    /// </summary>
    public class ReceiverCSVFileInt: ReceiverCSVFile<int>
    {
        public ReceiverCSVFileInt(string fileName, Encoding encoding): base(fileName,encoding)
        {
        }

    }
}
