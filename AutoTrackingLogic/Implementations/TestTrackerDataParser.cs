using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoTrackingLogic.Implementations
{
    class TestTrackerDataParser : ITrackerDataParser
    {
        #region ITrackerDataParser Members

        public TrackerData Parse(byte[] data)
        {
            int i=0;
            while (i < 1000)
            {
                i++;
            }

            Thread.Sleep(10);
            Random random = new Random();

            return new TrackerData()
            {
                Altitude = random.Next(0, 60),
                DateTime = DateTime.Now,
                Latitude = random.Next(0, 60) ,
                Longitude = random.Next(0, 60),
                PhoneNumber = "+79222265432",
                Velocity = random.Next(0, 100)
            };
        }

        #endregion
    }
}
