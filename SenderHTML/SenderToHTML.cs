﻿using RazorCompile;
using SenderToFile;
using StankinsInterfaces;
using System;
using System.Text;
using MediaTransform;
namespace SenderHTML
{
    public class Sender_HTMLText: SenderMediaToFile
    {
        public Sender_HTMLText(string outputFileName, string text) :base(outputFileName,new MediaTransformStringToText(text))
        {
            
        }
    }
    public class Sender_HTMLRazor : SenderMediaToFile
    {
        public string ViewFileName { get; set; }
        
        public Sender_HTMLRazor(string viewFileName,string outputFileName) 
            : base(outputFileName, new MediaTransformRazor(viewFileName))
        {
            this.ViewFileName = viewFileName;
        
        }
       

        public static string DefaultExport()=> @"@using System.Linq;
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
