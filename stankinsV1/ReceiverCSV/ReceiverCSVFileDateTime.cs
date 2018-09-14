using System;
using System.Text;

namespace ReceiverCSV
{
    /// <summary>
    /// CSV receiver for last value of DateTime data type.
    /// </summary>
    public class ReceiverCSVFileDateTime : ReceiverCSVFile<DateTime>
    {
        public ReceiverCSVFileDateTime(string fileName, Encoding encoding) : base(fileName, encoding)
        {
        }

    }
}
