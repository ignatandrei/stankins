using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReceiverCSV;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestVariables
    {
        [TestMethod]
        public async Task TestVariableMax()
        {
            return;
            #region arrange
            var dir = Path.Combine(AppContext.BaseDirectory, "TestVariableMax");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);

            var sb = new StringBuilder();
            sb.AppendLine("nr,model,Track_number");
            sb.AppendLine("1,Ford,B325ROS");
            sb.AppendLine("2,Audi,PL654CSM");
            sb.AppendLine("3,BMW,B325DFH");
            sb.AppendLine("4,Ford,B325IYS");
            var file = Path.Combine(dir, "cars.csv");
            File.WriteAllText(file, sb.ToString());
            SimpleJob sj = new SimpleJob();
            sj
                .AddReceiver(new ReceiverCSVFileInt(file, Encoding.UTF8))
                .AddTransformer(new TransformIntoVariable("MaxCars", GroupingFunctions.Max, "nr"))

            ;
            sj.RuntimeParameters = new RuntimeParameter[1];
            sj.RuntimeParameters[0] = new RuntimeParameter();
            sj.RuntimeParameters[0].VariableName = "MaxCars";
            //sj.RuntimeParameters[0].NameObjectsToApplyTo = new string[] { "MyReceiver" };

            #endregion
            #region act

            await sj.Execute();


            #endregion
            #region assert
            #endregion

        }
    }
}

