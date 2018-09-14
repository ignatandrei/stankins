using MediaTransform;
using StankinsInterfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenderCSV
{
    /// <summary>
    /// TODO:
    /// 
    /// 2. File.Write or File.Append
    /// 3. For null what to write?
    /// 4. For big data - to write in chunks  - size
    /// 5. encoding
    /// </summary>
    public class SenderToCSV : ISend
    {

        
        public IRow[] valuesToBeSent { private get; set; }
        public string CSVFileName{ get; set; }
        public SenderToCSV(string fileName)
        {
            this.CSVFileName = fileName;
        }
        
        
        public async Task Send()
        {


            var csvMedia = new MediaTransformCSV();
            csvMedia.valuesToBeSent = this.valuesToBeSent;
            await csvMedia.Run();
            var buffer = Encoding.UTF8.GetBytes(csvMedia.Result);

            using (var fs = new FileStream(CSVFileName, FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.None, buffer.Length, true))
            {
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }


        }
    }
}
