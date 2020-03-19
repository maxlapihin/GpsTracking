using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using TestUdpSender.Properties;

namespace TestUdpSender
{
    public class EmulatorPacketCreator:IPacketCreator 
    {
        #region IPacketCreator Members

        static DateTime start = DateTime.Now;

        /// <summary>
        /// Тестовый протокол обмена данными
        /// </summary>
        /// <returns></returns>
        public byte[] CreatePacket()
        {
            Double x1 = Settings.Default.X1;
            double x2 = Settings.Default.X2;
            Double y1 = Settings.Default.Y1;
            double y2 = Settings.Default.Y2;
            int t1 = Settings.Default.T;
            int deviceId = Settings.Default.DeviceID;

            //EMULATOR;[DeviceId];[Longitude];[Latitude];[Altitude];[Velocity];[PhoneNumber];[DateTime]
            DateTime now = DateTime.Now;
            double speedx = (x2 - x1) / (t1);
            double speedy = (y2 - y1) / (t1);
            Double deltaT = (now - start).TotalMinutes;
            double x = x1 + speedx * deltaT;
            double y = y1 + speedy * deltaT;
            NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
            string result = string.Format(nfi, "EMULATOR;{0};{1};{2};{3};{4};{5};{6}", deviceId, x, y, 1, new Random().Next(30,120), "892222222", now.ToUniversalTime());

            if ((x > x2 || y > y2) || (x < x1 || y < y1))
            {
                start = DateTime.Now;
            }

            return Encoding.ASCII.GetBytes(result);
        }

        #endregion
    }
}
