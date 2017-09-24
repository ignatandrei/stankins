using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderToFile
{
    public class SenderMediaSerialize<T>:ISend
    {
        public SenderMediaSerialize(ISerializeData serializeData,string key, IFilterTransformTo<T> transform)
        {
            SerializeData = serializeData;
            Key = key;
            Transform = transform;
        }

        public ISerializeData SerializeData { get; set; }
        public string Key { get; set; }
        public IFilterTransformTo<T> Transform { get; }
        public IRow[] valuesToBeSent { set; get; }
        public string Name { get ; set ; }

        public async Task Send()
        {
            Transform.valuesToBeSent = valuesToBeSent;
            await Transform.Run();
            //for date time, think about iso format yyyy-MM-dd 
            SerializeData.SetValue(Key, Transform.Result.ToString());
        }
    }
    /// <summary>
    /// TODO: encoding
    /// File.Write or File.Append
    /// For null what to write?
    /// For big data - to write in chunks  - size
    /// </summary>
    public class SenderMediaToFile: ISend
    {
        public string Name { get; set; }
        public IRow[] valuesToBeSent { protected get; set; }
        public string FileName { get; set; }
        public IFilterTransformToString[] media { get; set; }
        public IFilterTransformToByteArray[] MediaArray { get;set; }
        public SenderMediaToFile(string outputFileName, params IFilterTransformToString[] media )
        {
            this.FileName = outputFileName;
            this.media = media;
            this.Name = $"send to {Path.GetFileName(outputFileName)} ";
        }
        //public SenderMediaToFile(IFilterTransformToString media, string outputFileName)
        //{
        //    this.FileName= outputFileName;
        //    this.media = media;
        //    this.Name = $"send to {Path.GetFileName(outputFileName)} ";
        //}
        public SenderMediaToFile(string outputFileName,params IFilterTransformToByteArray[] transform)
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
            var bufferList = new List<byte>();
            if (media != null)
            {
                foreach (var item in media)
                {
                    item.valuesToBeSent = this.valuesToBeSent;
                    await item.Run();
                    if (item.Result?.Length == 0)
                        continue;
                    bufferList.AddRange(Encoding.UTF8.GetBytes(item.Result));
                }
                
            }
            if (MediaArray != null)
            {
                foreach (var item in MediaArray)
                {
                    item.valuesToBeSent = this.valuesToBeSent;
                    await item.Run();
                    if (item.Result?.Length == 0)
                        continue;
                    bufferList.AddRange(item.Result);
                }
            }
            var buffer = bufferList.ToArray();
            using (var fs = new FileStream(FileName, FileMode.Append,
                FileAccess.Write, FileShare.None, buffer.Length, true))
            {
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }


        }
    }
}
