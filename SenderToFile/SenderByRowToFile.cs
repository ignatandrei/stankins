using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderToFile
{
    public class SenderByRowToFile:ISend
    {
        public FileMode FileMode { get; set; }
        public SenderByRowToFile(string keyNameFile, string extensionFile,string rowDataKey)
        {
            KeyNameFile = keyNameFile;
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
            foreach (var item in valuesToBeSent)
            {
                string nameFile = item.Values[KeyNameFile]?.ToString();
                //TODO: normalize name file
                if (addExtension)
                    nameFile +=  ExtensionFile;


                var buffer = Encoding.UTF8.GetBytes(item.Values[RowDataKey]?.ToString());
                using (var fs = new FileStream(nameFile, FileMode,
                FileAccess.Write, FileShare.Write, buffer.Length, true))
                {
                    await fs.WriteAsync(buffer, 0, buffer.Length);
                }

            }
        }
        public string Name { get; set; }
        public string KeyNameFile { get; set; }
        public string ExtensionFile { get; set; }
        public string RowDataKey { get; set; }
    }
}
