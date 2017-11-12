using StanskinsImplementation;
using System.Text;

namespace ReceiverCSV
{
    public class ReceiverCSVWholeFile: ReceiverCSVFile<FakeComparable>
    {
        public ReceiverCSVWholeFile(string fileName, Encoding encoding) : base(fileName, encoding)
        {
        }

    }
}
