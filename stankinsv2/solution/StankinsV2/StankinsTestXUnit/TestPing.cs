using FluentAssertions;
using Stankins.Alive;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StankinsTestXUnit
{
    public class TestPing
    {
        [Fact]
        public async Task TestLocalHost()
        {
            #region arrange
            var r = new ReceiverPing("127.0.0.1");
            #endregion
            #region act
            var dt= await r.TransformData(null);
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
