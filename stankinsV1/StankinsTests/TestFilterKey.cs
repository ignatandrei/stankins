using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using Shouldly;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestFilterKey
    {
        [TestMethod]
        public async Task TestFilterKeyEqualsRemove()
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
            IFilter f = new FilterRemoveItemsWithKey("nrfiles", FilterType.Equal);
            f.valuesRead = r.valuesRead;
            await f.Run();
            #endregion
            #region assert
            bool haveFolder = false;
            foreach (var item in r.valuesRead)
            {
                if (haveFolder)
                    break;

                var keys = item.Values.Keys.ToArray();

                if (keys.Contains("nrfiles"))
                    haveFolder = true;
            }
            haveFolder.ShouldBeTrue();

            f.valuesTransformed.ShouldNotBeNull();
            f.valuesTransformed.Length.ShouldBeGreaterThan(0);
            foreach(var item in f.valuesTransformed)
            {
                var keys = item.Values.Keys.ToArray();
                keys.ShouldNotContain("nrfiles");
            }
            
            #endregion
        }

        [TestMethod]
        public async Task TestFilterKeyEqualsRemains()
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
            IFilter f = new FilterRetainItemsWithKey("nrfiles", FilterType.Equal);
            f.valuesRead = r.valuesRead;
            await f.Run();
            #endregion
            #region assert
            bool haveFolder = false;
            foreach (var item in r.valuesRead)
            {
                if (haveFolder)
                    break;

                var keys = item.Values.Keys.ToArray();

                if (keys.Contains("nrfiles"))
                    haveFolder = true;
            }
            haveFolder.ShouldBeTrue();

            f.valuesTransformed.ShouldNotBeNull();
            f.valuesTransformed.Length.ShouldBeGreaterThan(0);
            foreach (var item in f.valuesTransformed)
            {
                var keys = item.Values.Keys.ToArray();
                keys.ShouldContain("nrfiles");
            }

            #endregion
        }
    }
}
