using ReceiverFile;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReceiverCSV
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ReceiverCSVFile<T> : ReceiverFileFromStorage<T>
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
        
        List<IRowReceive> listOfData;
        protected override async Task ProcessText(string text)
        {
            if ((text?.Length ?? 0) == 0)
                return;
            foreach (var item in text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var textLine = item.Replace("\r", "");
                var row = textLine.Split(new string[] { "," }, StringSplitOptions.None);
                if (CSVHeaderLine != null && row.Length > CSVHeaderLine.Length)
                {
                    if (textLine.Contains("\""))
                    {
                        if ((textLine.Replace("\"", "").Length - textLine.Length) % 2 == 0)
                        {
                            row = Regex.Split(textLine, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        }
                    }
                    if (textLine.Contains("'"))
                    {
                        if ((textLine.Replace("'", "").Length - textLine.Length) % 2 == 0)
                        {
                            row = Regex.Split(textLine, ",(?=(?:[^']*'[^']*')*[^']*$)");
                        }
                    }
                }
                if (CSVHeaderLine == null)
                {
                    CSVHeaderLine = row;
                    continue ;
                }
                

                
                RowRead obj = new RowRead();
                
                for (int columns = 0; columns <Math.Min( row.Length,CSVHeaderLine.Length); columns++)
                {
                    obj.Values.Add(CSVHeaderLine[columns], row[columns]);
                }
                listOfData.Add(obj);
            }
            return ;
        }
        
    }
}
