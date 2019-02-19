using FluentAssertions;
using Stankins.FileOps;
using Stankins.HTML;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Stankins.AnalyzeSolution;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("ReceiverFromSolution", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverFromSolution
    {
        [Scenario]
        [Example(@"Assets/TestSolutionAnalyzer/TestSolutionAnalyzer.sln", 5)]
        public void TestSimpleSln(string filepath,int numberTables)
        {

            IDataToSent data=null;
            
            $"receiving the file {filepath} ".w(async () =>
            {
                data= await new ReceiverFromSolution(filepath).TransformData(null);
            });
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} tables".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(numberTables);
            });
            $"the solution table has 1 row".w(() =>
                data.FindAfterName("solutions").Value.Rows.Count.Should().Be(1));
            $"the project able has 1 row".w(() =>
                data.FindAfterName("projects").Value.Rows.Count.Should().Be(1));




        }
    }
}
