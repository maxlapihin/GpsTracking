using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTrackingLogic.Data;

namespace AutoTrackingLogic.Implementations
{
    public class MeiligaoDataParser : ITrackerDataParser
    {

        #region ITrackerDataParser Members
        /// <summary>
        /// little indian to big endian
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        ushort GetShort(byte[] data, int index)
        {

            byte[] temp = new byte[2];
            Array.Copy(data, index, temp, 0, 2);
            Array.Reverse(temp);
            return BitConverter.ToUInt16(temp, 0);
        }

        byte[] GetBytes(byte[] data, int index, int length)
        {
            //little indian to big endian
            byte[] temp = new byte[length];
            Array.Copy(data, index, temp, 0, length);
            Array.Reverse(temp);
            return temp;
        }

        private static List<ushort> _validCommands = new List<ushort>() { 0x9955, 0x9999 };

        public TrackerData Parse(byte[] data)
        {
            //получим длину пакета
            ushort length = GetShort(data, 2);

            if (length != data.Length)
            {
                throw new TrackerDataParserException(string.Format("Ожидаемая длина пакета {0}, полученная {1}", length, data.Length));
            }

            ushort crc = CRCode(data, (ushort)(data.Length - 4));
            byte[] crcBytes = BitConverter.GetBytes(crc);

            if (crcBytes[0] != data[length - 3] && crcBytes[1] != data[length - 4])
            {
                throw new TrackerDataParserException("Неправильная CRC пакета");
            }



            // выясним id контроллера
            string controllerId = string.Empty;
            for (int i = 4; i < 11; i++)
            {
                controllerId += string.Format("{0:X}", data[i]);
            }
            controllerId = controllerId.TrimEnd('F');

            // получим команду
            ushort command = GetShort(data, 11);
            if (!_validCommands.Contains(command))
            {
                throw new TrackerDataParserException(string.Format("Получена неверная команда: {0:X} ", command));
            }

            // получим строку данных 
            string info = Encoding.ASCII.GetString(data, 13, length - 13 - 4);
            // разобъем на подстроки через запятую
            string[] infoParts = info.Split(new[] { ',' });

            int hours, minutes, seconds, milliseconds;
            int year, month, day;

            int latDegrees, latMinutes, latDecimalMinutes;
            int longDegrees, longMinutes, longDecimalMinutes;
            string longitudeStr, latitudeStr;

            //hhmmss.dd,S,xxmm.dddd,<N|S>,yyymm.dddd,<E|W>,s.s,h.h,ddmmyy
            // получим все поля данных
            GetTime(infoParts[0], out hours, out minutes, out seconds, out milliseconds);
            GetLatitude(infoParts[2], out latDegrees, out latMinutes, out latDecimalMinutes);
            latitudeStr = infoParts[3];
            GetLongitude(infoParts[4], out longDegrees, out longMinutes, out longDecimalMinutes);
            longitudeStr = infoParts[5];
            GetDate(infoParts[8], out year, out month, out day);


            Latitude latitude = new Latitude()
            {
                Degrees = (byte)latDegrees,
                Minutes = (byte)latMinutes,
                MinutesDecimal = latDecimalMinutes / 10000f,
                Str = latitudeStr
            };

            Longitude longitude = new Longitude()
            {
                Degrees = (byte)longDegrees,
                Minutes = (byte)longMinutes,
                MinutesDecimal = longDecimalMinutes / 10000f,
                Str = longitudeStr
            };



            return new TrackerData()
            {
                Altitude = -1,
                DateTime = new DateTime(year, month, day, hours, minutes, seconds, milliseconds),
                Latitude = latitude.DoubleRepresentation,
                Longitude = longitude.DoubleRepresentation,
                Velocity = -1,
                PhoneNumber = controllerId
            };

        }

        private const string _FORMAT = "L dd mm ss";
        private const char _DOT = '.';
        #region Helpers


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

        #endregion

        /// <summary>
        /// подсчет CRC
        /// </summary>
        /// <param name="pcBlock"></param>
        /// <param name="len"></param>
        /// <returns></returns>

        ushort CRCode(byte[] pcBlock, ushort len)
        {

            byte[] blockCopy = new byte[pcBlock.Length];
            Array.Copy(pcBlock, blockCopy, pcBlock.Length);

            ushort crc = CRCCalculator.Calculate(blockCopy, len);
            return crc;

        }

        #endregion
    }

    public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }

    public class Crc16Ccitt
    {
        const ushort poly = 4129;
        ushort[] table = new ushort[256];
        ushort initialValue = 0;

        public ushort ComputeChecksum(byte[] bytes, int length)
        {
            ushort crc = this.initialValue;
            for (int i = 0; i < length; i++)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes, int length)
        {
            ushort crc = ComputeChecksum(bytes, length);
            return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
        }

        public Crc16Ccitt(InitialCrcValue initialValue)
        {
            this.initialValue = (ushort)initialValue;
            ushort temp, a;
            for (int i = 0; i < table.Length; i++)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; j++)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }
    }


    public class CRCCalculator
    {
        // Fields
        private const int CCNET_CRC_POLY = 0x8408;



        // Methods
        public static ushort Calculate(byte[] data, long dataLength)
        {
            Crc16Ccitt c = new Crc16Ccitt(InitialCrcValue.NonZero1);
            return c.ComputeChecksum(data, (int)dataLength);
        }
    }

}

