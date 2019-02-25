using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Stankins.Amazon;
using Stankins.AssemblyLoad;
using Stankins.Interfaces;
using Stankins.Version;
using Stankins.XML;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("AssemblyReceiver", "")]
    [Trait("ExternalDependency", "0")]
    public class TestAssemblyReceiver
    {
        [Scenario]
       
        public void TestAssemblyReceiverCurrentAssembly ()
         {
            IReceive receiver = null;
            IDataToSent data=null;
            DataTable dtContent= null;
            string ass=Assembly.GetExecutingAssembly().FullName;
            $"When I create the {nameof(AssemblyReceiver )} with {ass} ".w(() => receiver = new AssemblyReceiver(ass));

            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"and types".w(() =>
            {
                
                dtContent = data.DataToBeSentFurther[0];
                dtContent.Rows.Count.Should().BeGreaterThan(1);


            });
            


         }
    }
}
