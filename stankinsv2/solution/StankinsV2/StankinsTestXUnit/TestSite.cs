using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("site", "ping")]
    [Trait("ExternalDependency", "Azure Deploy exists")]
    public class TestSite
    {
        [Fact]
        public async Task PingLiveSite()
        {
            var h = new HttpClient();
            var str= await h.GetStringAsync(@"https://azurestankins.azurewebsites.net/api/utils/ping");
            var dt = DateTime.ParseExact(str, "s", System.Globalization.CultureInfo.InvariantCulture);
            var dateNow = DateTime.UtcNow;
            var diff = DateTime.UtcNow - dt;
            diff.TotalSeconds.Should().BeInRange(-2, 2);
        }

    }
}
