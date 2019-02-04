using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Stankins.AnalyzeSolution;
using Xbehave;
using Xunit;
using System.IO;
using System.Linq;

namespace StankinsTestXUnit
{
    [Trait("SolutionReceiverTest", "")]
    [Trait("ExternalDependency", "0")]
    public class SolutionReceiverTest
    {
        //[Scenario]
        //public void TestSol()
        //{
        //    var root = Path.GetPathRoot(Directory.GetCurrentDirectory());
        //    //var f = Directory.GetFiles(root, "StankinsV2.sln", SearchOption.AllDirectories).First();
        //    var f = FindAllFilesWithoutRaisingException(root, "StankinsV2.sln").First();
        //    var r = new ReceiverFromSolution(f);
        //    "Load".w(async () => await r.TransformData(null));

        //}
        private string[] FindAllFilesWithoutRaisingException(string folder, string file)
        {
            var lst = new List<string>();
            foreach (var f in Directory.GetFiles(folder, file, SearchOption.TopDirectoryOnly))
            {
                lst.Add(f);
            }
            foreach (string subDir in Directory.GetDirectories(folder))
            {
                try
                {
                    foreach (var f in FindAllFilesWithoutRaisingException(subDir, file))
                    {
                        lst.Add(f);
                    }
                }
                catch
                {
                    //do nothing
                }
            }
            return lst.ToArray();
        }



        [Trait("FilterColumnDataGreaterThanLength", "")]
        [Trait("ExternalDependency", "0")]
        public class FilterColumnDataGreaterThanLengthTest
        {
            [Scenario]
            [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003{NewLine}Mercedes, 2003", 3, 1)]
            public void TestSimpleCSV(string fileContents, int NumberRows, int NumberRowsAfterFilter)
            {
                IReceive receiver = null;
                IDataToSent data = null;
                var nl = Environment.NewLine;
                fileContents = fileContents.Replace("{NewLine}", nl);
                $"When I create the receiver csv for the content {fileContents}".w(() =>
                    receiver = new ReceiverCSVText(fileContents));
                $"And I read the data".w(async () => data = await receiver.TransformData(null));
                $"Then should be a data".w(() => data.Should().NotBeNull());
                $"With a table".w(() =>
                {
                    data.DataToBeSentFurther.Should().NotBeNull();
                    data.DataToBeSentFurther.Count.Should().Be(1);
                });
                $"The number of rows should be {NumberRows}".w(() =>
                    data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
                $"And when I filter".w(async () =>
                    data = await new FilterColumnDataGreaterThanLength("Car", 5).TransformData(data));
                $"Then should be a data".w(() => data.Should().NotBeNull());
                $"With a table".w(() =>
                {
                    data.DataToBeSentFurther.Should().NotBeNull();
                    data.DataToBeSentFurther.Count.Should().Be(1);
                });
                $"The number of rows should be {NumberRowsAfterFilter}".w(() =>
                    data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRowsAfterFilter));


            }
        }
    }
}
