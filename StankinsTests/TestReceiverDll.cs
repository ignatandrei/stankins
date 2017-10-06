using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverDll;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    
    public class TestReceiverDll
    {
        [TestMethod]
        public async Task TestReceiveDllImplementation()
        {
#region arrange
            var path = AppContext.BaseDirectory;
            string fileName = Path.Combine(path, "StanskinsImplementation.dll");
            #endregion
#region act
            var rd = new ReceiverFromDllPlain(fileName);
            rd.LoadInterfaces = true;
            rd.LoadBaseClasses = true;
            await rd.LoadData();
            #endregion
            #region assert
            rd.valuesRead.ShouldNotBeNull();
            rd.valuesRead.Length.ShouldBeGreaterThan(1);

            #endregion


        }
    }
}
