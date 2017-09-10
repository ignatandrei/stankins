using MediaTransform;
using SenderToFile;
using StankinsInterfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SenderHTML
{
    public class Sender_HierarchicalVizJob: Sender_HierarchicalViz<MediaTransformDotJob>
    {
        public Sender_HierarchicalVizJob(string outputFileName, string label) : base(outputFileName, label)
        {

        }
    }
        public class Sender_HierarchicalVizFolder:Sender_HierarchicalViz<MediaTransformDotFolderAndFiles>
    {
        public Sender_HierarchicalVizFolder(string outputFileName, string label) :base(outputFileName,label)
        {

        }
        
    }
    public class Sender_HierarchicalViz<T>: SenderMediaToFile
        where T: MediaTransformDotHierarchical,new()
    {
        
        public Sender_HierarchicalViz(string outputFileName, string label) 
            : base(outputFileName,new T())
        {
            Label = label;
            VizUrl = "https://github.com/mdaines/viz.js/releases/download/v1.8.0/viz.js";
        }
        public string  VizUrl{ get; set; }

        public string Label { get; set; }        
        public override async Task Send()
        {
            
            var dot = new T();
            dot.LabelField = Label;
            dot.valuesToBeSent = this.valuesToBeSent;            
            await dot.Run();
            var res = dot.Result.Replace("\r", "").Replace("\n", "");
            string data = $"" +
                
                $" <script src='{VizUrl}'></script>" +
                @"
<script>" +
$"var result = Viz('{res}', {{format: 'png-image-element' }});" +
@"
document.body.appendChild(result);
</script>
"
            //+$"{dot.Result}"
            ;
            
            File.AppendAllText(FileName, data);
        }


//        public string DefaultExport() =>
//         @"@using System.Linq;
//@using StankinsInterfaces;
//@model StankinsInterfaces.IRow[]

//Number Rows: @Model.Length
//@{
//	bool showTable=(Model.Length>0);
//	if(!showTable){
//		return;
//    };
//	var FieldNames= Model[0]
//                .Values
//                .Select(it => it.Key).ToArray();
//}
//<table>
//<thead>
//<tr>
//<td>ID</td>
//@foreach(var col in FieldNames){

//<td>
//@col
//</td>

//}
//<td>Parent</td>
//</thead>

//<tbody>
//@foreach(var item in Model){
//    var m=item as IRowReceiveHierarchicalParent;

//<tr>
//<td>@m.ID</td>
//@foreach(var col in FieldNames){
//<td>
//@item.Values[col]
//</td>
//}
//<td>
//@if(m.Parent != null){
//    <text>@m.Parent.ID</text>
//}
//</td>
//</tr>

//}
//<tbody>
//</table>";
    }
}
