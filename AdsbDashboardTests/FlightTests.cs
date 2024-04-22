using AdsbDashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace AdsbDashboardTests
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

        [Fact]
        public void CreatePlaneEqualsTest()
        {
            Plane p1 = new()
            {
                ModeS = "4d236a"

            };
            Plane p2 = new()
            {
                ModeS = "4d236a",
                LastSeen = new DateTime(2024,1,1)
            };

            Plane p3 = new()
            {
                ModeS = "471f7a"
            };
            string p4 = "4d236a";

            bool shouldEqual = p1.Equals(p2) && (p1 == p2);
            bool shouldEqual2 = p1.Equals(p4);

            bool shoutNotequal = p2.Equals(p3);
            bool shoutNotequal2 = p3.Equals(p4);


            Assert.True(p1 != null);
            Assert.True(shouldEqual);
            Assert.True(shouldEqual2);
            Assert.False(shoutNotequal);
            Assert.False(shoutNotequal2);
        }
    }
}
