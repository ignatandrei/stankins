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
    /// Receiver for *.csv files.
    /// </summary>
    /// <typeparam name="T">Data type of last value.</typeparam>
    public abstract class ReceiverCSVFile<T> : ReceiverFileFromStorage<T>
         where T : IComparable<T>
    {

        /// <summary>
        /// Initializes a new instance of the ReceiverCSVFile class.
        /// </summary>
        /// <param name="fileName">Source file.</param>
        /// <param name="encoding">Encoding used to read data from file.</param>
        public ReceiverCSVFile(string fileName, Encoding encoding): base(fileName,false,encoding)
        {
            listOfData = new List<IRowReceive>();
            this.EndReadFile += ReceiverCSVFile_EndReadFile;
        }

        /// <summary>
        /// Called at the end of reading file to fill valuesRead with data read from file.
        /// </summary>
        /// <param name="sender">Current object.</param>
        /// <param name="e">EventArgs.</param>
        private void ReceiverCSVFile_EndReadFile(object sender, EventArgs e)
        {
            valuesRead = listOfData.ToArray();
        }

        /// <summary>
        /// Array of strings with file header.
        /// </summary>
        string[] CSVHeaderLine;

        /// <summary>
        /// Temp storage for data read from file. <seealso cref="ReceiverCSVFile_EndReadFile"/>
        /// </summary>
        List<IRowReceive> listOfData;
        /// <summary>
        /// Split a line read from file into separate values and fills listOfData. <seealso cref="listOfData"/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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
