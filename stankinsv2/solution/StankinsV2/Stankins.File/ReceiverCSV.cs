using StankinsCommon;
using Stankins.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.File
{
    public class ReceiverCSV : IStreamingReceive<string>
    {
        public ReceiverCSV(string nameFile, Encoding encoding,char separator)
        {
            NameFile = nameFile;
            Encoding = encoding;
            Separator = separator;
        }

        public string NameFile { get;  }
        public Encoding Encoding { get; }
        public char Separator { get; }

        public async Task<bool> Initialize()
        {
            return await Task.FromResult(true);
        }


        public async Task<string[]> StreamData()
        {
            var file = new ReadFileToString
            {
                FileEnconding = this.Encoding,
                FileToRead = this.NameFile
            };
            var data = await file.LoadData();
            var splitLines = data.Split(Separator);
            return await Task.FromResult(splitLines);
        }
    }
}