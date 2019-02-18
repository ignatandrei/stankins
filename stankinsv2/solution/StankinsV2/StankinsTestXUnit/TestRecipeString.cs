using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stankins.Interpreter;
using Stankins.XML;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("RecipeFromString", "")]
    [Trait("ExternalDependency", "0")]
    public class TestRecipeFromString
    {
        [Scenario]
        [Example("Assets/XML/nbrfxrates.xml",32)]
        public void SimpleXMLFromString(string fileName,int NumberRows)
        {
            string s = $"ReceiverXML file={fileName} xpath=//*[name()='Rate']";

            IReceive receiver = null;
            IDataToSent data=null;

            $"Given the recipe {s} ".w( () =>
            {
                File.Exists(fileName).Should().BeTrue();
            });
            $"When I create the r{nameof(RecipeFromString)} ".w(() =>
                receiver = new RecipeFromString(s));

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
         [Example("Assets/XML/nbrfxrates.xml",32,"currency", "Value","4.7383")]
        public void ObtainExchangeRates(string fileName,int NumberRows, string currencyName,string ValueName, string valueEur)
        {
            string s = $"ReceiverXML file={fileName} xpath=//*[name()='Rate']";
            s+=Environment.NewLine;
            s+=$"TransformerXMLToColumn columnName=OuterXML xPath=//@{currencyName} newColumnName={currencyName}";
            s+=Environment.NewLine;
            s+=$"FilterRemoveColumn nameColumn=OuterXML";
            s+=Environment.NewLine;
            s+=$"FilterRemoveColumn nameColumn=Name";
            s+=Environment.NewLine;
            s+=$"SenderExcel fileName=a.xlsx";

            IReceive receiver = null;
            IDataToSent data=null;

            $"Given the recipe {s} ".w( () =>
            {
                File.Exists(fileName).Should().BeTrue();
            });
            $"When I create the r{nameof(RecipeFromString)} ".w(() =>
                receiver = new RecipeFromString(s));

            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"the table should contain {nameof(currencyName)}".w(() =>
                data.Metadata.Columns.FirstOrDefault(it => it.Name == currencyName).Should().NotBeNull());

            
            $"the table should contain {nameof(ValueName)}".w(() =>
                data.Metadata.Columns.FirstOrDefault(it => it.Name == ValueName).Should().NotBeNull());

            $"and the data should contain for EUR {nameof(valueEur)}".w(() =>
            {
                DataView dv = new DataView(data.DataToBeSentFurther[0]);
                dv.RowFilter = $"{currencyName}='EUR'";
                dv.Count.Should().Be(1);
                var row=dv[0].Row;
                row[ValueName].ToString().Should().Be(valueEur);

            });
            $"and should be just 2 columns".w(()=>data.DataToBeSentFurther[0].Columns.Count.Should().Be(2));
        } 

       
    }
}
