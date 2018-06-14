using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenderToFile
{
    public class SenderByRowToFile:ISend
    {
        public FileMode FileMode { get; set; }
        public SenderByRowToFile(string keyNameFile,string keylineNr, string rowDataKey, string extensionFile)
        {
            KeyNameFile = keyNameFile;
            KeylineNr = keylineNr;
            ExtensionFile = extensionFile;
            RowDataKey = rowDataKey;
        }
        public IRow[] valuesToBeSent { set; get; }

        public string Result { get; set; }

        public async Task Send()
        {
            if (FileMode == 0)
                FileMode = FileMode.Create;
            bool addExtension = (!string.IsNullOrWhiteSpace(ExtensionFile));
            var vals = valuesToBeSent
                    .ToList();
            vals.Sort((a, b) =>
            {
                var val1 = a.Values[KeyNameFile]?.ToString();
                var val2 = b.Values[KeyNameFile]?.ToString();
                var diff = val1.CompareTo(val2);
                if (diff != 0)
                    return diff;

                val1 = a.Values[KeylineNr]?.ToString();
                val2 = b.Values[KeylineNr]?.ToString();
                if(int.TryParse(val1, out var v1) && int.TryParse(val2, out var v2))
                {
                    return v1.CompareTo(v2);
                }
                return val1.CompareTo(val2);
            });
                    

            foreach (var item in vals)
            {
                string nameFile = item.Values[KeyNameFile]?.ToString();
                //TODO: normalize name file
                if (addExtension)
                    nameFile +=  ExtensionFile;


                var buffer = Encoding.UTF8.GetBytes(item.Values[RowDataKey]?.ToString()+Environment.NewLine);
                using (var fs = new FileStream(nameFile, FileMode,
                FileAccess.Write, FileShare.Write, buffer.Length, true))
                {
                    await fs.WriteAsync(buffer, 0, buffer.Length);
                }

            }
        }
        public string Name { get; set; }
        public string KeyNameFile { get; set; }
        public string KeylineNr { get; }
        public string ExtensionFile { get; set; }
        public string RowDataKey { get; set; }
    }
}
