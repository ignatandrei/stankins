using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReceiverCSV;
using SenderToFile;
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
    public class SimpleJobConditionalTransformersTest
    {
        public IRow[] GenerateData()
        {
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "Andrei" + i,
                        ["LastName"] = "Ignat" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }
            return rows.ToArray();
        }
        [TestMethod]
        public async Task SimpleJobConditionalTransformersTestSimpleReadCSV()
        {

            #region ARRANGE
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("model,Track_number,buyYear");
            sb.AppendLine("Ford,B325ROS,1990");
            sb.AppendLine("Audi,PL654CSM,2004");
            sb.AppendLine("BMW,B325DFH,2005");
            sb.AppendLine("Ford,B325IYS,2007");

            string filename = "mycsv.csv";

            File.WriteAllText(filename, sb.ToString());
            //define a receiver
            var receiverCSV = new ReceiverCSVFileInt(filename, Encoding.ASCII);

            //define a sender to csv for all records
            var senderAllToCSV = new Sender_CSV("myAll.csv");


            //define a filter for audi 
            var filterAudi = new FilterComparableEqual(typeof(string), "Audi", "model");
            //define a sender just for audi
            var senderCSVAudi = new Sender_CSV("myAudi.csv");

            //define a filter to transform the buyYear to string
            
            var buyYearTOString = new TransformerFieldStringInt("buyYear", "NewBuyYear");
            //define a filter for year>2000
            var filterYear2000 = new FilterComparableGreat(typeof(int), 2000, "NewBuyYear");
            //define a sender the year > 2000 to csv
            var sender2000CSV = new Sender_CSV("my2000.csv");
            //define a sender the year > 2000 to json
            var sender2000JSon = new Sender_JSON("my2000.js");
            #endregion
            #region ACT




            var cond = new SimpleJobConditionalTransformers();
            //add a receiver
            cond.Receivers.Add(0, receiverCSV);
           
            //add a sender to csv for all records
            cond.AddSender(senderAllToCSV);

            //add a filter for audi and a sender just for audi
            cond.Add(filterAudi, senderCSVAudi);


            //add a filter to transform the buyYear to string
            //and then fiter for year>2000
            cond.Add(buyYearTOString, filterYear2000);
            //send the year> 2000 to csv
            cond.Add(filterYear2000, sender2000CSV);
            //send the year >2000 to json
            cond.Add(filterYear2000, sender2000JSon);

            await cond.Execute();
            #endregion
            #region ASSERT
            Assert.IsTrue(File.Exists("myAudi.csv"));
            var lines = File.ReadAllLines("myAudi.csv");
            Assert.AreEqual(2, lines.Length);

            Assert.IsTrue(File.Exists("myAll.csv"));
            lines = File.ReadAllLines("myAll.csv");
            Assert.AreEqual(5, lines.Length);


            Assert.IsTrue(File.Exists("my2000.csv"));
            lines = File.ReadAllLines("my2000.csv");
            Assert.AreEqual(4, lines.Length);

            Assert.IsTrue(File.Exists("my2000.js"));


            #endregion

        }
    }
}
