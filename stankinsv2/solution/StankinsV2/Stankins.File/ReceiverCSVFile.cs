using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.File
{
    public class ReceiverCSVFile : ReceiveCSV<ReceiverCSV>
    {
        public string FileCSV { get; }
        public Encoding Encoding { get; }
        public ReceiverCSVFile (CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverCSVFile);
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);
            FileCSV = GetMyDataOrThrow<string>(nameof(FileCSV));
            

        }
        public ReceiverCSVFile(string fileCSV, Encoding encoding, bool firstLineHasColumnNames, char columnSeparator, char lineSeparator) : this(new CtorDictionary()
            {
                {nameof(fileCSV),fileCSV },
                {nameof(encoding),encoding },
                {nameof(columnSeparator),columnSeparator },
                {nameof(lineSeparator),lineSeparator},
                {nameof(firstLineHasColumnNames),firstLineHasColumnNames }

            })
        {

        }
        

        public override Task<IMetadata> TryLoadMetadata()
        {
            //TODO : read first line
            throw new NotImplementedException();
        }

        public override ReceiverCSV Create()
        {
            var s = new ReceiverCSV(this.FileCSV, this.Encoding, this.LineSeparator);
            return s;
        }

        public override string NameTable()
        {
            return Path.GetFileNameWithoutExtension(this.FileCSV);
        }
    }
}