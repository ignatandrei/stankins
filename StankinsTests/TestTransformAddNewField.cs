using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transformers;
using Shouldly;
namespace StankinsTests
{

    [TestClass]
    public class TestTransformAddNewField
    {
        [TestMethod]
        public async Task TestTransformAddFieldFormatter()
        {
            #region arange
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var row = new Mock<IRow>();

                row.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "John" + i
                    }
                );

                rows.Add(row.Object);
            }
            #endregion

            #region act
            var transform = new TransformAddFieldFormatter("Out", "{ID} is {FirstName}");
            
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            #endregion

            #region assert
            for (int i = 0; i < nrRows; i++)
            {
                var item = transform.valuesTransformed[i];
                item.Values.ShouldContainKeyAndValue("Out", string.Format("{0} is {1}",rows[i].Values["ID"],rows[i].Values["FirstName"]));
            }

            #endregion
        }
        [TestMethod]
        public async Task TestTransformAddNewFieldSimple()
        {
            #region arange
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var row = new Mock<IRow>();

                row.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "John" + i
                    }
                );

                rows.Add(row.Object);
            }
            #endregion

            #region act
            var transform = new TransformAddNewField("LastName");
            transform.Value = "test";
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            #endregion

            #region assert
            foreach (var item in transform.valuesTransformed)
            {
                
                item.Values.ShouldContainKeyAndValue("LastName", "test");
            }

            #endregion
        }


        [TestMethod]
        public async Task TestTransformAddNewFieldID()
        {
            #region arange
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var row = new Mock<IRow>();

                row.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                       
                        ["FirstName"] = "John" + i
                    }
                );

                rows.Add(row.Object);
            }
            #endregion

            #region act
            var transform = new TransformAddNewField("ID");
            transform.Increment = true;
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            #endregion

            #region assert
            int nr=0;
            foreach (var item in transform.valuesTransformed)
            {              
                item.Values.ShouldContainKeyAndValue("ID", nr);
                nr++;
            }

            #endregion
        }
    }
}