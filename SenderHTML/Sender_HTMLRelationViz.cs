using MediaTransform;
using RazorCompile;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderHTML
{
    public class Sender_HTMLRelationViz: Sender_Viz<MediaTransformDotRelational>
    {
        
        public Sender_HTMLRelationViz(string label, string outputFileName):base(outputFileName)
        {
            this.Name = $"sender html viz to {outputFileName}";
            
            
            
            Label = label;
            
        }

        
        public string Label { get; set; }

        public override void AddToMedia(MediaTransformDotRelational dot)
        {
            dot.LabelField = Label;
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
