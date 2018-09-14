using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using Shouldly;
using StankinsInterfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestFiltersHierarchical
    {
        [TestMethod]
        public async Task TestFilterHierarchicalFolders()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;

            string filename = Path.Combine(dir, "a.html");
            if (File.Exists(filename))
                File.Delete(filename);


            string fileNameToWrite = Guid.NewGuid().ToString("N") + ".txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");

            #endregion
            #region act
            IReceive r = new ReceiverFolderHierarchical(dir, "*.txt");
            await r.LoadData();
            var fi = new FilterForFoldersHierarchical();
            fi.valuesRead = r.valuesRead;
            await fi.Run();
            #endregion
            #region assert
            foreach(var item in fi.valuesTransformed)
            {
                item.Values.ShouldContainKeyAndValue("RowType","folder");
            }
            #endregion

        }

        [TestMethod]
        public async Task TestFilterHierarchicalFiles()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;
            dir = UtilsIO.DeleteCreateFolder(Path.Combine(dir, "TestFilterHierarchicalFiles"));
            string filename = Path.Combine(dir, "a.html");
            if (File.Exists(filename))
                File.Delete(filename);


            string fileNameToWrite = Guid.NewGuid().ToString("N") + ".txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");

            #endregion
            #region act
            IReceive r = new ReceiverFolderHierarchical(dir, "*.txt");
            await r.LoadData();
            var fi = new FilterForFilesHierarchical();
            fi.valuesRead = r.valuesRead;
            await fi.Run();
            #endregion
            #region assert
            foreach (var item in fi.valuesTransformed)
            {
                item.Values.ShouldContainKeyAndValue("RowType", "file");
            }
            #endregion

        }
    }
}
