using FluentAssertions;
using Stankins.Alive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("ReceiverProcess", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverProcess
    {
        [Fact]
        public async Task TestPingLocalHost()
        {

            #region arrange
            var args = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                args = "127.0.0.1 -n 1";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                args = "127.0.0.1 -c 1";
            }
            var r = new ReceiverProcessAlive("ping", args);
            #endregion
            #region act
            var dt = await r.TransformData(null);
            #endregion
            #region assert
            dt.Should().NotBeNull();
            dt.DataToBeSentFurther.Should().NotBeNull();
            dt.DataToBeSentFurther.Count.Should().Be(1);
            dt.DataToBeSentFurther.First().Value.Should().NotBeNull();
            dt.DataToBeSentFurther.First().Value.Rows.Count.Should().BeGreaterOrEqualTo(1);
            var row = dt.DataToBeSentFurther.First().Value.Rows[0];
            row["IsSuccess"].Should().Be(true);
            #endregion
        }
    }
}