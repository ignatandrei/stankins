using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsDataWeb
{
    public class Writables
    {
        private readonly string folder;

        public Writables(string folder)
        {
            this.folder = folder;
        }
        public IEnumerable<KeyValuePair<string,string>> Files()
        {
            return Directory.GetFiles(folder)
                .Select(it=>new KeyValuePair<string,string>(Path.GetFileName(it),System.IO.File.ReadAllText(it)))
                .ToArray();
        }
        public async Task<string> GetFileContents(string nameFile)
        {
            nameFile=Path.Combine(folder,nameFile);
            return await File.ReadAllTextAsync(nameFile);
        }
        public async Task WriteFileContents(string nameFile,string content)
        {
            nameFile=Path.Combine(folder,nameFile);
            await File.WriteAllTextAsync(nameFile,content);
        }
        public void DeleteFile(string nameFile)
        {
            nameFile=Path.Combine(folder,nameFile);
            File.Delete(nameFile);
        }
    }
}
