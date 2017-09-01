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
        public string Name { get; set; }
        public IRow[] valuesToBeSent { protected get; set; }
        public string FileName { get; set; }
        public IFilterTransformToString media { get; set; }
        public IFilterTransformToByteArray MediaArray { get;set; }

        public SenderMediaToFile(IFilterTransformToString media, string outputFileName)
        {
            this.FileName= outputFileName;
            this.media = media;
            this.Name = $"send to {Path.GetFileName(outputFileName)} ";
        }
        public SenderMediaToFile(IFilterTransformToByteArray transform, string outputFileName)
        {
            this.FileName = outputFileName;
            
            MediaArray = transform;
            this.Name = $"send to {Path.GetFileName(outputFileName)} ";
        }
        public virtual async Task Send()
        {

            if (valuesToBeSent == null || valuesToBeSent.Length == 0)
                return;
            if (media == null && MediaArray == null)
                return;
            byte[] buffer=null;
            if (media != null)
            {
                media.valuesToBeSent = this.valuesToBeSent;
                await media.Run();
                if (media.Result == null || media.Result.Length == 0)
                    return;
                buffer = Encoding.UTF8.GetBytes(media.Result);
            }
            if (MediaArray != null)
            {
                MediaArray.valuesToBeSent = this.valuesToBeSent;
                await MediaArray.Run();
                if (MediaArray.Result == null || MediaArray.Result.Length == 0)
                    return;
                buffer = MediaArray.Result;
            }

            using (var fs = new FileStream(FileName, FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.None, buffer.Length, true))
            {
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }


        }
    }
}
