using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFile;
using ReceiverFileSystem;
using ReceiverJSON;
using Shouldly;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestTransformerApplyReceiver
    {
        [TestMethod]
        public async Task TestReceiverFileFromStorageLines()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;
            dir = Path.Combine(dir, "TestReceiverFileFromStorageLines");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            string fileNameToWrite = "andrei.txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            if (File.Exists(fullNameFile))
                File.Delete(fullNameFile);

            File.WriteAllText(fullNameFile, "andrei ignat"+Environment.NewLine+"aaa");
            #endregion
            #region act
            var readFile = new ReceiverFileFromStorageLines(fullNameFile,Encoding.UTF8);
            readFile.ReadAllFirstTime = false;
            await readFile.LoadData();
            #endregion
            #region assert
            readFile.valuesRead.ShouldNotBeNull();
            readFile.valuesRead.Length.ShouldBe(2, "2  lines above");            
#endregion
        }
        [TestMethod]
        public async Task TestTransformerApplyReceiverFiles()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;
            dir = UtilsIO.DeleteCreateFolder(Path.Combine(dir, "TestTransformerApplyReceiverFiles"));
            string filename = Path.Combine(dir, "a.html");
            if (File.Exists(filename))
                File.Delete(filename);


            string fileNameToWrite = Guid.NewGuid().ToString("N") + ".txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat" + Environment.NewLine + "aaa");
            var readFile = new ReceiverFileFromStorageLines(fullNameFile, Encoding.UTF8);
            readFile.ReadAllFirstTime = false;
            #endregion
            #region act
            IReceive r = new ReceiverFolderHierarchical(dir, "*.txt");
            await r.LoadData();
            var fi = new FilterForFilesHierarchical();
            fi.valuesRead = r.valuesRead;
            await fi.Run();

            var transformerApplyReceiver = new TransformerApplyReceiver(readFile, "FileToRead", "FullName");
            transformerApplyReceiver.valuesRead = fi.valuesTransformed;
            await transformerApplyReceiver.Run();

            #endregion
            #region assert
            transformerApplyReceiver.valuesTransformed.ShouldNotBeNull();
            transformerApplyReceiver.valuesTransformed.Length.ShouldBe(2, "2  lines above");
            #endregion
        }
    }
}
