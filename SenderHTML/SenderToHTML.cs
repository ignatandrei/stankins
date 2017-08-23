using MediaTransform;
using RazorCompile;
using SenderToFile;
using StankinsInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderHTML
{
    public class Sender_HTMLHierarchicalViz: Sender_HTML
    {
        public Sender_HTMLHierarchicalViz(string viewFileName, string outputFileName, string label) 
            : base(viewFileName, outputFileName)
        {
            Label = label;
            VizUrl = "https://github.com/mdaines/viz.js/releases/download/v1.8.0/viz.js";
        }
        public string  VizUrl{ get; set; }

        public string Label { get; set; }

        public override async Task Send()
        {
            await base.Send();
            var dot = new MediaTransformDot(Label);
            dot.valuesToBeSent = this.valuesToBeSent;
            await dot.Run();
            var res = dot.Result.Replace("\r", "").Replace("\n", "");
            string data = $"<script src='{VizUrl}'></script>"+
                @"
<script>"+
$"var result = Viz('{res}', {{format: 'png-image-element' }});"+
@"
document.body.appendChild(result);
</script>
";
            
            File.AppendAllText(FileName, data);
        }
    }
    public class Sender_HTML : SenderMediaToFile
    {
        public string ViewFileName { get; set; }
        
        public Sender_HTML(string viewFileName,string outputFileName) 
            : base(new MediaTransformRazor(viewFileName), outputFileName)
        {
            this.ViewFileName = viewFileName;
        
        }
        public Sender_HTML(string outputFileName) : this(DefaultExport, outputFileName)
        {

        }

        public static string DefaultExport= @"@using System.Linq;
@model StankinsInterfaces.IRow[]

Number Rows: @Model.Length
@{
	bool showTable=(Model.Length>0);
	if(!showTable){
		return;
    };
	var FieldNames= Model[0]
                .Values
                .Select(it => it.Key).ToArray();
}
<table>
<thead>
<tr>
@foreach(var col in FieldNames){

<td>
@col
</td>

}
</thead>
</tr>
<tbody>
@foreach(var item in Model){
<tr>
@foreach(var col in FieldNames){
<td>
@item.Values[col]
</td>
}
</tr>
}
<tbody>
</table>";
    }
}
