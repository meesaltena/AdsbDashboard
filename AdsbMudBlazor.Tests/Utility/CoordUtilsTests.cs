using Xunit;
using AdsbMudBlazor.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = Xunit.Assert;
using Microsoft.Extensions.Options;
using AdsbMudBlazor.Models;

namespace AdsbMudBlazor.Utility.Tests
{
    public class CoordUtilsTests
    {
        [Theory]
        [InlineData(0, 0, 1, 0, 111200)]
        [InlineData(52.56679961098458, 5.466062091225237, 52.481083429587976, 1.7630341443588327, 250680)]
        public void GetDistanceTest(double lat1, double lon1, double lat2, double lon2, double expected)
        {
            IOptions<FeederOptions> options = Options.Create<FeederOptions>(new FeederOptions()
            {
                FeederLat = lat1,
                FeederLong = lon1
            });

            CoordUtils coordUtils = new CoordUtils(options);

            double distance = coordUtils.GetDistance(lat1, lon1, lat2, lon2);
            double distance2 = coordUtils.GetDistance(lat2, lon2);

            Assert.Equal(distance, distance2);

            double tolerancePercent = 0.5;
            double deviation = 100 - ((expected / distance) * 100);
            bool valid = Math.Abs(deviation) <= tolerancePercent;

            Assert.True(valid);
        }
    }
}