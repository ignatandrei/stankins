using StankinsInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderToFile
{
    /// <summary>
    /// TODO: encoding
    /// File.Write or File.Append
    /// For null what to write?
    /// For big data - to write in chunks  - size
    /// </summary>
    public abstract class SenderMediaToFile: ISend
    {
        public IRow[] valuesToBeSent { private get; set; }
        public string FileName { get; set; }
        public IFilterTransformToString media { get; set; }
        public SenderMediaToFile(IFilterTransformToString media, string fileName)
        {
            this.FileName= fileName;
            this.media = media;
        }
        public async Task Send()
        {


            media.valuesToBeSent = this.valuesToBeSent;
            await media.Run();
            var buffer = Encoding.UTF8.GetBytes(media.Result);

            using (var fs = new FileStream(FileName, FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.None, buffer.Length, true))
            {
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }


        }
    }
}
