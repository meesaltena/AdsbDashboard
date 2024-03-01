using AdsbMudBlazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace AdsbMudBlazorTests
{
    public class FlightTests
    {
        [Fact]
        public void CreateFlightEqualsTest()
        {
            Flight f1 = GetFlight();
            Flight f2 = GetFlight();
            Flight f3 = GetFlight();
            f3.Distance = 400;
            Flight f4 = GetFlight();
            f4.Groundspeed = 200;

            bool shouldEqual = f1.Equals(f2) && f1.Equals(f3);
            bool shoutNotequal2 = f1.Equals(f4);

            Assert.True(shouldEqual);
            Assert.False(shoutNotequal2);
        }

        private Flight GetFlight()
        {
            return new()
            {
                ModeS = "484cc2",
                Callsign = "FOOBAR",
                Alt = "10000",
                Groundspeed = 300,
                Squawk = "5000",
                Lat = 0.0,
                Long = 0.0,
            };
        }
    }
}
