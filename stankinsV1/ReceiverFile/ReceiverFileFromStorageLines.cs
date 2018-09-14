using StankinsInterfaces;
using System;
using System.Text;
using System.Threading.Tasks;
using StanskinsImplementation;
using System.Collections.Generic;

namespace ReceiverFile
{
    public class ReceiverFileFromStorageLines : ReceiverFileFromStorage<FakeComparable>
    {
        public ReceiverFileFromStorageLines(string fileToRead, Encoding fileEnconding) : base(fileToRead, true, fileEnconding)
        {
            Name = "read lines from " + fileToRead;
            this.StartReadFile += ReceiverFileFromStorageLines_StartReadFile;
            this.EndReadFile += ReceiverFileFromStorageLines_EndReadFile;
        }
        long line;
        List<IRowReceive> values;
        private void ReceiverFileFromStorageLines_EndReadFile(object sender, EventArgs e)
        {
            valuesRead = values.ToArray();
        }


        private void ReceiverFileFromStorageLines_StartReadFile(object sender, EventArgs e)
        {
            line = 1;
            values = new List<IRowReceive>();
        }

        protected override async Task ProcessText(string text)
        {
            var rr = new RowRead();
            rr.Values.Add("lineNr", line++);
            rr.Values.Add("name", FileToRead);
            rr.Values.Add("line", text);
            values.Add(rr);
            await Task.CompletedTask;
            
        }
    }
}
