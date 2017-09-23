using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using Shouldly;
using StankinsInterfaces;
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
    public class TestPBX
    {
        [TestMethod]
        public async Task TestPBXData()
        {
            var dir = AppContext.BaseDirectory;
            var dirPBX = Path.Combine(dir, "PBX");
            IReceive r = new ReceiverFolderHierarchical(dirPBX, "*.log");
            IFilter filterFiles = new FilterForFilesHierarchical();
            ITransform transformLines = new TransformerFileToLines() { TrimEmptyLines = true };
            var trDateRegex = new TransformRowRegex(@"^Date:\ (?<datePBX>.{23}).*?$", "text");
            var trToDate = new TransformerFieldStringToDate("datePBX", "NewDatePBX", "yyyy/MM/dd HH:mm:ss.fff");
            var si = new SimpleJob();
            si.Receivers.Add(0,r);
            si.FiltersAndTransformers.Add(0,filterFiles);
            si.FiltersAndTransformers.Add(1, transformLines);
            si.FiltersAndTransformers.Add(2, trDateRegex);
            si.FiltersAndTransformers.Add(3, trToDate);
            await si.Execute();

            filterFiles.valuesTransformed.Length.ShouldBe(2,  "just two files after first filter");
            transformLines.valuesTransformed.Length.ShouldBe(77251);
            var d = transformLines.valuesTransformed.Select(it => it.Values["FullName"]).Distinct().ToArray();
            d.Length.ShouldBe(2, "two files after reading contents");
            //transformGroupingFiles.valuesTransformed.Length.ShouldBe(2);
            trDateRegex.valuesRead.Count(it => it.Values.ContainsKey("datePBX")).ShouldBeGreaterThan(0);
            trToDate.valuesRead.Count(it => it.Values.ContainsKey("NewDatePBX")).ShouldBeGreaterThan(0);

        }
    }
}
