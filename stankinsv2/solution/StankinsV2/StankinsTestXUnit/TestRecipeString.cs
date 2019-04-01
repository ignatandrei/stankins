using FluentAssertions;
using Stankins.Interfaces;
using Stankins.Interpreter;
using System;
using System.Data;
using System.IO;
using System.Linq;
using Xbehave;
using Xunit;
namespace StankinsTestXUnit
{
    [Trait("RecipeFromString", "")]
    
    public class TestRecipeFromString
    {
        [Scenario]
        [Example("Assets/XML/nbrfxrates.xml", 32)]
        [Trait("ExternalDependency", "0")]
        public void SimpleXMLFromString(string fileName, int NumberRows)
        {
            string s = $"ReceiverXML file={fileName} xpath=//*[name()='Rate']";

            IReceive receiver = null;
            IDataToSent data = null;

            $"Given the recipe {s} ".w(() =>
           {
               File.Exists(fileName).Should().BeTrue();
           });
            $"When I create the r{nameof(RecipeFromString)} ".w(() =>
                receiver = new RecipeFromString(s));

            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
        }

        [Scenario]
        [Example("Assets/XML/nbrfxrates.xml", 32, "currency", "Value", "4.7383")]
        [Trait("ExternalDependency", "0")]
        public void ObtainExchangeRates(string fileName, int NumberRows, string currencyName, string ValueName, string valueEur)
        {
            string s = $"ReceiverXML file={fileName} xpath=//*[name()='Rate']";
            s += Environment.NewLine;
            s += $"#just a comment";
            s += Environment.NewLine;
            s += $"TransformerXMLToColumn columnName=OuterXML xPath=//@{currencyName} newColumnName={currencyName}";
            s += Environment.NewLine;
            s += $"FilterRemoveColumn nameColumn=OuterXML";
            s += Environment.NewLine;
            s += $"FilterRemoveColumn nameColumn=Name";
            s += Environment.NewLine;
            s += $"SenderOutputExcel fileName=a.xlsx";
            s += Environment.NewLine;
            s += $"SenderOutputToFolder folderToSave=exports addKey=false";


            IReceive receiver = null;
            IDataToSent data = null;

            $"Given the recipe {s} ".w(() =>
           {
               File.Exists(fileName).Should().BeTrue();
           });
            $"When I create the r{nameof(RecipeFromString)} ".w(() =>
                receiver = new RecipeFromString(s));

            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"the table should contain {nameof(currencyName)}".w(() =>
                data.Metadata.Columns.FirstOrDefault(it => it.Name == currencyName).Should().NotBeNull());


            $"the table should contain {nameof(ValueName)}".w(() =>
                data.Metadata.Columns.FirstOrDefault(it => it.Name == ValueName).Should().NotBeNull());

            $"and the data should contain for EUR {nameof(valueEur)}".w(() =>
            {
                DataView dv = new DataView(data.DataToBeSentFurther[0])
                {
                    RowFilter = $"{currencyName}='EUR'"
                };
                dv.Count.Should().Be(1);
                DataRow row = dv[0].Row;
                row[ValueName].ToString().Should().Be(valueEur);

            });
            $"and should be just 2 columns".w(() => data.DataToBeSentFurther[0].Columns.Count.Should().Be(2));
        }


        [Scenario]
        [Example("Assets/XML/nbrfxrates.xml", 32, "currency", "Value", "4.7383")]
        [Trait("ExternalDependency", "0")]
        public void ObtainExchangeRatesInRecipesFromFile(string fileName, int NumberRows, string currencyName, string ValueName, string valueEur)
        {
            string s = $"ReceiverXML file={fileName} xpath=//*[name()='Rate']";
            s += Environment.NewLine;
            s += $"#just a comment";
            s += Environment.NewLine;
            s += $"TransformerXMLToColumn columnName=OuterXML xPath=//@{currencyName} newColumnName={currencyName}";
            s += Environment.NewLine;
            s += $"FilterRemoveColumn nameColumn=OuterXML";
            s += Environment.NewLine;
            s += $"FilterRemoveColumn nameColumn=Name";
            s += Environment.NewLine;
            s += $"SenderOutputExcel fileName=a.xlsx";
            s += Environment.NewLine;
            s += $"SenderOutputToFolder folderToSave=exports addKey=false";
            System.IO.File.WriteAllText("myRecipe", s);

            IReceive receiver = null;
            IDataToSent data = null;

            $"Given the recipe {s} ".w(() =>
            {
                File.Exists(fileName).Should().BeTrue();
            });
            $"When I create the r{nameof(RecipeFromFilePath)} ".w(() =>
                receiver = new RecipeFromFilePath("myRecipe"));

            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"the table should contain {nameof(currencyName)}".w(() =>
                data.Metadata.Columns.FirstOrDefault(it => it.Name == currencyName).Should().NotBeNull());


            $"the table should contain {nameof(ValueName)}".w(() =>
                data.Metadata.Columns.FirstOrDefault(it => it.Name == ValueName).Should().NotBeNull());

            $"and the data should contain for EUR {nameof(valueEur)}".w(() =>
            {
                DataView dv = new DataView(data.DataToBeSentFurther[1])
                {
                    RowFilter = $"{currencyName}='EUR'"
                };
                dv.Count.Should().Be(1);
                DataRow row = dv[0].Row;
                row[ValueName].ToString().Should().Be(valueEur);

            });
            $"and should be just 2 columns".w(() => data.DataToBeSentFurther[0].Columns.Count.Should().Be(2));
        }
        [Scenario]
        [Example("BNR")]
        [Example("ChuckNorris")]        
        [Trait("ExternalDependency", "0")]
        public void TestRecipeFind(string nameRecipe)
        {
            $"trying to find {nameRecipe}".w(() =>
            {
                RecipeFromString r = RecipeFromString.FindRecipe(nameRecipe);
                r.Should().NotBeNull();
            });
        }

        

    }
}
