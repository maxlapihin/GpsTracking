using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;

namespace AutoTrackingLogic.Implementations
{
    class EmulatorTrackerDataParser : ITrackerDataParser
    {
        #region ITrackerDataParser Members

        public TrackerData Parse(byte[] data)
        {
            //EMULATOR;[DeviceId];[Longitude];[Latitude];[Altitude];[Velocity];[PhoneNumber];[DateTime]

            string packet = Encoding.ASCII.GetString(data);

            string[] splittedData = packet.Split(';'); 

            if (splittedData.Length!=8 )
                throw new ArgumentException("Invalid packet");

            int deviceId     = int.Parse(splittedData[1]);
            double longitude = double.Parse(splittedData[2], CultureInfo.InvariantCulture.NumberFormat);
            double latitude  = double.Parse(splittedData[3], CultureInfo.InvariantCulture.NumberFormat);
            double altitude  = double.Parse(splittedData[4], CultureInfo.InvariantCulture.NumberFormat);
            double velocity  = double.Parse(splittedData[5], CultureInfo.InvariantCulture.NumberFormat);
            string phone     = splittedData[6];
            DateTime dt = DateTime.Now;//DateTime.Parse(splittedData[7]);

            return new TrackerData()
            {
                DeviceId=deviceId,
                Altitude = altitude,
                DateTime = dt,
                Latitude = latitude,
                Longitude = longitude,
                PhoneNumber = phone,
                Velocity = velocity
            };
        }

        #endregion
    }
}
