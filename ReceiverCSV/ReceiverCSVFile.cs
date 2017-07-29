using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Threading.Tasks;

namespace ReceiverCSV
{
    public class ReceiverCSVFile : IReceive
    {
        public string FileName { get; set; }
        public ReceiverCSVFile(string fileName)
        {
            FileName = fileName;
        }

        public IRowReceive[] valuesRead { get; set; }

        public async Task LoadData()
        {
            string[] CSVlines = System.IO.File.ReadAllLines(FileName);
            if (CSVlines?.Length == 0)
            {
                //LOG: there are no data 
                return;
            }
            var CSVHeaderLine = CSVlines[0].Split(new string[] {  "," }, StringSplitOptions.None);
            IRowReceive[] valuesToBeRead = new IRowReceive[CSVlines.Length - 1];
            

           
            for (int nrLine = 1; nrLine < CSVlines.Length; nrLine++)
            {
                var line = CSVlines[nrLine];
                var row = line.Split(new string[] {  "," }, StringSplitOptions.None);
                RowRead obj = new RowRead();
                for (int columns = 0; columns < row.Length; columns++)
                {
                    obj.Values.Add(CSVHeaderLine[columns], row[columns]);

                }
                valuesToBeRead[nrLine-1] = obj;
                
            }
            valuesRead = valuesToBeRead;

        }
    }
}
