using System;
using System.Text;

namespace ReceiverCSV
{
    public class ReceiverCSVFileDateTime : ReceiverCSVFile<DateTime>
    {
        public ReceiverCSVFileDateTime(string fileName, Encoding encoding) : base(fileName, encoding)
        {
        }

    }
}
