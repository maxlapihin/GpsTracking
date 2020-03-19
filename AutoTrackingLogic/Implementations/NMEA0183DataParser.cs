using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTrackingLogic.Data;

namespace AutoTrackingLogic.Implementations
{
    public class NMEA0183DataParser : ITrackerDataParser
    {
        public TrackerData Parse(byte[] data)
        {
            //$GPRMC,hhmmss.ss ,A,GGMM.MM  ,P,gggmm.mm  ,J,v.v ,b.b  ,ddmmyy,x.x,n,m*hh<CR><LF>
            //$GPRMC,125504.049,A,5542.2389,N,03741.6063,E,0.06,25.82,200906,   ,*3B



            // получим строку данных 
            string info = Encoding.ASCII.GetString(data);

            string[] infoParts = info.Split(new[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in infoParts)
            {
                if (!item.StartsWith("$GPRMC"))
                {
                    continue;
                }
                return GetNMEA(item);
            }

            return null;

        }

        private TrackerData GetNMEA(string info)
        {
            // разобъем на подстроки через запятую
            string[] infoParts = info.Split(new[] { ',' });

            int hours, minutes, seconds, milliseconds;
            int year, month, day;

            int latDegrees, latMinutes, latDecimalMinutes;
            int longDegrees, longMinutes, longDecimalMinutes;
            string longitudeStr, latitudeStr;


            // получим все поля данных
            GetTime(infoParts[1], out hours, out minutes, out seconds, out milliseconds);
            GetLatitude(infoParts[3], out latDegrees, out latMinutes, out latDecimalMinutes);
            latitudeStr = infoParts[4];
            GetLongitude(infoParts[5], out longDegrees, out longMinutes, out longDecimalMinutes);
            longitudeStr = infoParts[6];
            GetDate(infoParts[9], out year, out month, out day);


            Latitude latitude = new Latitude()
            {
                Degrees = (byte)latDegrees,
                Minutes = (byte)latMinutes,
                Seconds  = (byte)(latDecimalMinutes * 60 / 100000 ),
                Str = latitudeStr
            };

            Longitude longitude = new Longitude()
            {
                Degrees = (byte)longDegrees,
                Minutes = (byte)longMinutes,
                Seconds = (byte)(longDecimalMinutes * 60 / 100000),
                Str = longitudeStr
            };

            return new TrackerData()
            {
                Altitude = -1,
                DateTime = new DateTime(year, month, day, hours, minutes, seconds, milliseconds),
                Latitude = latitude.DoubleRepresentation,
                Longitude = longitude.DoubleRepresentation,
                Velocity = -1,
                PhoneNumber = ""
            };
        }

        private const string _FORMAT = "L dd mm ss";
        private const char _DOT = '.';

        void GetTime(string str, out int hours, out int minutes, out int seconds, out int milliseconds)
        {
            string[] timeParts = str.Split('.');

            hours = int.Parse(timeParts[0].Substring(0, 2));
            minutes = int.Parse(timeParts[0].Substring(2, 2));
            seconds = int.Parse(timeParts[0].Substring(4, 2));

            milliseconds = int.Parse(timeParts[1]);
        }

        void GetDate(string str, out int year, out int month, out int day)
        {
            day = int.Parse(str.Substring(0, 2));
            month = int.Parse(str.Substring(2, 2));
            year = int.Parse("20" + str.Substring(4, 2));
        }

        void GetLatitude(string str, out int degrees, out int minutes, out int decimalMinutes)
        {
            string[] timeParts = str.Split(_DOT);

            degrees = int.Parse(timeParts[0].Substring(0, 2));
            minutes = int.Parse(timeParts[0].Substring(2, 2));

            decimalMinutes = int.Parse(timeParts[1]);
        }

        void GetLongitude(string str, out int degrees, out int minutes, out int decimalMinutes)
        {
            string[] timeParts = str.Split(_DOT);

            degrees = int.Parse(timeParts[0].Substring(0, 3));
            minutes = int.Parse(timeParts[0].Substring(3, 2));

            decimalMinutes = int.Parse(timeParts[1]);
        }
    }
}


