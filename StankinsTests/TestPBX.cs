using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
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

            var si = new SimpleJob();
            si.Receivers.Add(0,r);
            si.FiltersAndTransformers.Add(0,filterFiles);
            await si.Execute();

        }
    }
}
