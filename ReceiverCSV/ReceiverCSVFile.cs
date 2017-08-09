using ReceiverFile;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverCSV
{
    /// <summary>
    /// todo: make abstract and implement datetime+int
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReceiverCSVFile<T> : ReceiverFileFromStorage<T>
         where T : IComparable<T>
    {
        
        public ReceiverCSVFile(string fileName, Encoding encoding): base(fileName,false,encoding)
        {
            listOfData = new List<IRowReceive>();
            this.EndReadFile += ReceiverCSVFile_EndReadFile;
        }

        private void ReceiverCSVFile_EndReadFile(object sender, EventArgs e)
        {
            valuesRead = listOfData.ToArray();
        }

        string[] CSVHeaderLine;
        
        public IRowReceive[] valuesRead { get; set; }
        List<IRowReceive> listOfData;
        protected override Task ProcessText(string text)
        {
            if(CSVHeaderLine == null)
            {
                CSVHeaderLine=text.Split(new string[] { "," }, StringSplitOptions.None);
                return Task.CompletedTask;
            }
            var row = text.Split(new string[] { "," }, StringSplitOptions.None);
            RowRead obj = new RowRead();
            for (int columns = 0; columns < row.Length; columns++)
            {
                obj.Values.Add(CSVHeaderLine[columns], row[columns]);
            }
            listOfData.Add(obj);
            return Task.CompletedTask;
        }
        
    }
}
