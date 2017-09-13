using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReceiverFileSystem;
using SenderHTML;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderHTML
    {
        

        //public TestContext TestContext { get; set; }

        
        [TestMethod]
        public async Task TestSendHTMLData()
        {

            #region arange
            var dir = AppContext.BaseDirectory;
            dir = Path.Combine(dir, "TestSendHtmlData");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            string filename =Path.Combine(dir, "a.html");
            if (File.Exists(filename))
                File.Delete(filename);

            foreach( var item in Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories))
            {
                File.Delete(item);
            }
            //TODO:more files
            string fileNameToWrite = Guid.NewGuid().ToString("N") + ".txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");


            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "Andrei"+i,
                        ["LastName"] = "Ignat"+i
                    }
                );

                rows.Add(rowAndrei.Object);
            }


            

            var fileRazor = Path.Combine(dir, "my.cshtml");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(fileRazor,
 @"@using System.Linq;
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
</table>");

            #endregion
            #region act
            ISend sender = new Sender_HTMLRazor("TestSendHtmlData/" + Path.GetFileName(fileRazor), filename);
            sender.valuesToBeSent = rows.ToArray();
            await sender.Send();
            #endregion
            #region assert
            Assert.IsTrue(File.Exists(filename),$"file {filename} must exists in export csv");
            Assert.IsTrue(File.ReadAllText(filename).Contains($"Ignat{nrRows-1}"), "must contain data");
            
            #endregion
        }

        [TestMethod]
        public async Task TestSendHTMLDataHierarchical()
        {
            
            #region arange

            
            var dir = AppContext.BaseDirectory;
            dir = Path.Combine(dir, "TestSendDataHierarchical");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);

            string filename = Path.Combine(dir, "senderhtml.html");
            if (File.Exists(filename))
                File.Delete(filename);

            foreach (var item in Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories))
            {
                File.Delete(item);
            }

            string fileNameToWrite = "andrei.txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");

            fileNameToWrite = "ignat.txt";
            fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");

            var di=Directory.CreateDirectory(Path.Combine(dir, "2"));
            File.WriteAllText(Path.Combine(di.FullName, "2childfile.txt"), "test");

            di = Directory.CreateDirectory(Path.Combine(dir, "3"));
            File.WriteAllText(Path.Combine(di.FullName, "3childfile.txt"), "test");

            di = Directory.CreateDirectory(Path.Combine(di.FullName, "3childFolder"));

            IReceive r = new ReceiverFolder(dir, "*.txt");
            await r.LoadData();

            

            var fileRazor = Path.Combine(dir, "my.cshtml");

           

            File.WriteAllText(fileRazor,
 @"@using System.Linq;
@using StankinsInterfaces;
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
<td>ID</td>
@foreach(var col in FieldNames){

<td>
@col
</td>

}
<td>Parent</td>
</thead>

<tbody>
@foreach(var item in Model){
    var m=item as IRowReceiveHierarchicalParent;

<tr>
<td>@m.ID</td>
@foreach(var col in FieldNames){
<td>
@item.Values[col]
</td>
}
<td>
@if(m.Parent != null){
    <text>@m.Parent.ID</text>
}
</td>
</tr>

}
<tbody>
</table>");

            #endregion
            #region act
            ISend sender = new SyncSenderMultiple(
                new Sender_HTMLText(filename, "<html><body>"),
                new Sender_HTMLRazor("TestSendDataHierarchical/" + Path.GetFileName(fileRazor), filename),
                new Sender_HierarchicalVizFolder(filename, "Name"),
                new Sender_HTMLText(filename, "</body></html>")
                );

            
            sender.valuesToBeSent = r.valuesRead;
            await sender.Send();
            #endregion
            #region assert
            Assert.IsTrue(File.Exists(filename), $"file {filename} must exists in export hierarchical");
            Assert.IsTrue(File.ReadAllText(filename).Contains(fileNameToWrite), "must contain data");
            Assert.IsTrue(File.ReadAllText(filename).Contains("Viz("), "must contain viz ...");
            //System.Diagnostics.Process.Start("explorer.exe", filename);
            #endregion
        }
    }
}