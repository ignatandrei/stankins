using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Stankins.FileOps;
using Stankins.Interfaces;
using StankinsObjects;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
     [Trait("ExternalDependency","SqlServer")]
    public class TestSeparatorGo
    {
        [Scenario]
        [Trait("SplitAfterGO", "")]
        [Example("Assets/SQL/GoSep.txt","\nGO",3, new string[] { "select 1", "select 2", "select 3" })]
        public void TestSimpleSeparator(string fileName,string split, int NumberRows,string[] rows)
        {
            BaseObject receiver = null;
           
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName}".w(() =>
            {
                File.Exists(fileName).Should().BeTrue();
            });
            $"and I receive from file".w(async () =>
            {
                receiver = new ReceiverReadFileText(fileName);
                data = await receiver.TransformData(null);
            });
            $"and I split after {split} ".w(async () =>
            {
                receiver = new TransformSplitColumnAddRow("FileContents", "FileContents", split);
                data = await receiver.TransformData(data);
            });
            $"and should be {NumberRows} rows".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(1);
                var dt = data.DataToBeSentFurther.First().Value;
                dt.Rows.Count.Should().Be(NumberRows);
            });
            $"and the data should be correct".w(() =>
            {
                var dt = data.DataToBeSentFurther.First().Value;
                for (int i = 0; i < rows.Length; i++)
                {
                    DataRow dr = dt.Rows[i];
                    dr[0].ToString().Should().Contain(fileName);
                    dr[1].ToString().Should().Contain(rows[i]);

                }
            });

        }
    }
}
