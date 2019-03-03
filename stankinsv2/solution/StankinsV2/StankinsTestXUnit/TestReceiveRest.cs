using FluentAssertions;
using Stankins.FileOps;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Stankins.Rest;
using Xbehave;
using Xunit;
using static System.Environment;
using Stankins.Trello;

namespace StankinsTestXUnit
{
    [Trait("ReceiveRest", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiveRest
    {
        [Scenario]
        [Example("Assets/JSON/json1.txt",2)]
        [Example("Assets/JSON/json1Record.txt",1)]
        public void TestSimpleJSON(string fileName,int NumberRows)
        {
            IReceive receiver = null;
           
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName}".w(() =>
            {
                File.Exists(fileName).Should().BeTrue();
            });
            $"When I create the receiver csv for the {fileName}".w(() => receiver = new ReceiveRest(fileName));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));


        } 
        [Scenario]
        //[Example("Assets/JSON/trello.txt",3)]
        [Example("Assets/JSON/trelloAlphabet.txt",3,2)]
        //[Example("https://trello.com/b/SQYjpHEf.json",3)]
        public void TestTrelloJson(string fileName,int numberTables,int nrLists)
        {
            IReceive receiver = null;
           
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName}".w(() =>
            {
                //File.Exists(fileName).Should().BeTrue();
            });
            $"When I create the receiver trello for the {fileName}".w(() => receiver = new ReceiverFromTrello(fileName));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a {numberTables}:   tables: list, card , tables".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(numberTables);
            });
            $"With {nrLists} lists".w(() =>
            {
                var list=data.FindAfterName("list");
                list.Value.Rows.Count.Should().Be(nrLists);
            });
            //$"The number of rows should be {numberTables} tables".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));


        } 

    }
}
