using RazorCompile;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderHTML
{
    public class Sender_HTMLRelationViz : ISend
    {
        public string VizUrl { get; set; }
        public Sender_HTMLRelationViz(string rootFileName,string label, string outputFileName)
        {
            this.Name = "sender html viz";
            
            OutputFileName = outputFileName;
            RootFileName = rootFileName;
            Label = label;
            VizUrl = "https://github.com/mdaines/viz.js/releases/download/v1.8.0/viz.js";
        }

        
        public string OutputFileName { get; set; }
        public string RootFileName { get; set; }
        public string Label { get; set; }
        public IRow[] valuesToBeSent { get; set; }
        public string Name { get; set; }

        public async Task Send()
        {
            if (valuesToBeSent == null || valuesToBeSent.Length == 0)
                return;

            List<byte> buffer = new List<byte>();
            

            if (!File.Exists(RootFileName))
            {
                throw new FileNotFoundException($"root {RootFileName} must exists", RootFileName);
            }
            var mediaTransform = new MediaTransformRazor(RootFileName);
            mediaTransform.valuesToBeSent = this.valuesToBeSent;
            await mediaTransform.Run();
            if (!string.IsNullOrWhiteSpace(mediaTransform.Result))
                buffer.AddRange(Encoding.UTF8.GetBytes(mediaTransform.Result));

            //foreach (var item in valuesToBeSent)
            //{
            //    IRowReceiveRelation rr = item as IRowReceiveRelation;
            //    if (rr == null)
            //        continue;
            //    var b=await TransformRelation(rr,FolderNameWithRootAndRelation);
            //    if (b?.Length > 0)
            //    {
            //        buffer.AddRange(b);
            //    }
            //}
            var bytes = buffer.ToArray();
            using (var fs = new FileStream(OutputFileName, FileMode.OpenOrCreate,
              FileAccess.Write, FileShare.None, bytes.Length, true))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length);
            }

        }
        //async Task<byte[]> TransformRelation(IRowReceiveRelation rr,string folderRoot)
        //{
        //    if (rr == null)
        //        return null;
            
        //    var buffer = new List<byte>();
        //    foreach (var relation in rr.Relations.Keys)
        //    {
        //        var fileNameRelation = Path.Combine(FolderNameWithRootAndRelation, $"{relation}.cshtml");

        //        if (!File.Exists(fileNameRelation))
        //        {
        //            throw new FileNotFoundException($"{fileNameRelation} must exists", fileNameRelation);
        //        }
        //        var mediaTransform = new MediaTransformRazorTuple(fileNameRelation,rr);
        //        var valuesRelation = rr.Relations[relation].ToArray();
        //        mediaTransform.valuesToBeSent = valuesRelation;
        //        await mediaTransform.Run();
        //        if (!string.IsNullOrWhiteSpace(mediaTransform.Result))
        //            buffer.AddRange(Encoding.UTF8.GetBytes(mediaTransform.Result));
        //        foreach(var item in valuesRelation)
        //        {
        //            var itemRR = item as IRowReceiveRelation;
        //            if (itemRR == null)
        //                continue;

        //            var b=await TransformRelation(itemRR, folderRoot);
        //            if (b?.Length > 0)
        //                buffer.AddRange(b);

        //        }

        //    }
        //    return buffer.ToArray();
        //}
    }
}
