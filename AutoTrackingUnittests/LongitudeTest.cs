using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTrackingLogic.Data;

namespace AutoTrackingUnittests
{
    [TestFixture]
    public class LongitudeTest
    {
        [Test]
        public void Initialization()
        {
            Longitude longitude = new Longitude();
            longitude.Degrees = 0;
            longitude.Minutes = 60;
            longitude.Seconds = 60;
            longitude.Str = "W";

            Assert.AreEqual(1, longitude.Degrees);
            Assert.AreEqual(1, longitude.Minutes);
            Assert.AreEqual(0, longitude.Seconds);
        }

        [Test]
        public void Initialization2()
        {
            Longitude longitude = new Longitude();
            longitude.Degrees = 180;
            longitude.Minutes = 60;
            longitude.Seconds = 60;
            longitude.Str = "W";

            Assert.AreEqual(180, longitude.Degrees);
            Assert.AreEqual(0, longitude.Minutes);
            Assert.AreEqual(0, longitude.Seconds);
        }

        [Test]
        public void Initialization3()
        {
            Longitude longitude = new Longitude();
            longitude.Degrees = 75;
            longitude.Minutes = 30;
            longitude.MinutesDecimal = 0.25f;
            longitude.Str = "W";

            Assert.AreEqual(15, longitude.Seconds );

            //longitude.MinutesDecimal  = 0.25f;

            //Assert.AreEqual(15, longitude.Minutes );


        }

        [Test]
        public void Format1()
        {
            Longitude longitude = new Longitude();
            longitude.Degrees = 65;
            longitude.Minutes  = 20;
            longitude.Seconds = 10;
            longitude.Str = "W";

            Assert.AreEqual("W 65 20", longitude.ToString("L ddd mm"));
            Assert.AreEqual("W 65-10", longitude.ToString("L ddd-ss"));
            Assert.AreEqual("W_10|20|65", longitude.ToString("L_ss|mm|ddd"));
            Assert.AreEqual("W 65 20 10", longitude.ToString("L ddd mm ss"));
            Assert.AreEqual("W 65-20-10", longitude.ToString("L ddd-mm-ss"));
            Assert.AreEqual("W_65/20_10", longitude.ToString("L_ddd/mm_ss"));


            longitude.Degrees = 165;
            longitude.Minutes = 20;
            longitude.Seconds = 10;
            longitude.Str = "E";

            Assert.AreEqual("E 165 20 10", longitude.ToString("L ddd mm ss"));
            Assert.AreEqual("E 165-20-10", longitude.ToString("L ddd-mm-ss"));
            Assert.AreEqual("E_165/20_10", longitude.ToString("L_ddd/mm_ss"));

        }


    }
}
