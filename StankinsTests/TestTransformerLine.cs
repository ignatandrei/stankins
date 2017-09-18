using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using Shouldly;
using StanskinsImplementation;
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
    public class TestTransformerLine
    {
        [TestMethod]

        public async Task TestTransformerFileToLines()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;

            var folderSql = Path.Combine(dir, "SqlToExecute");
            var receiverFolder = new ReceiverFolderHierarchical(folderSql, "*.txt");

            #endregion
            #region act
            var transformer = new TransformerFileToLines();
            var j = new SimpleJob();
            j.Receivers.Add(0, receiverFolder);
            j.FiltersAndTransformers.Add(0, transformer);
            await j.Execute();
            #endregion
            #region assert
            transformer.valuesTransformed.ShouldNotBeNull();
            var files = transformer.valuesTransformed.GroupBy(it => it.Values["FullName"]).ToArray();
            files.Length.ShouldBeGreaterThan(1);
            var file1Len = files.First().Count();
            var file2Len = files.Last().Count();
            file1Len.ShouldBeGreaterThan(0);
            file2Len.ShouldBeGreaterThan(0);
            (file1Len + file2Len).ShouldBeGreaterThan(2);
            #endregion
        }
    }
}
