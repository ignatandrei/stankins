using RazorCompile;
using SenderToFile;
using StankinsInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderHTML
{
    
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
