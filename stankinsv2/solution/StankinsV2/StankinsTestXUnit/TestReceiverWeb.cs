using FluentAssertions;
using Stankins.Alive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("ReceiverWeb", "")]
    [Trait("AfterPublish", "0")]
    [Trait("ExternalDependency","1")]
    public class TestReceiverWeb
    {
        [Fact]
        public async Task TestAzureDeployWebSite()
        {
            #region arrange
            var r = new ReceiverWeb("https://azurestankins.azurewebsites.net/api/utils/ping");
            #endregion
            #region act
            var dt = await r.TransformData(null);
            #endregion
            #region assert
            dt.Should().NotBeNull();
            dt.DataToBeSentFurther.Should().NotBeNull();
            dt.DataToBeSentFurther.Count.Should().Be(1);
            dt.DataToBeSentFurther.First().Value.Should().NotBeNull();
            dt.DataToBeSentFurther.First().Value.Rows.Count.Should().Be(1);
            var row = dt.DataToBeSentFurther.First().Value.Rows[0];
            row["IsSuccess"].Should().Be(true);
            #endregion
        }
    }
}
