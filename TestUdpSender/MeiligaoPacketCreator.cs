using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace TestUdpSender
{
    public class MeiligaoPacketCreator:IPacketCreator 
    {
         // создание пакета для тестирования
        // пакет без HDOP | Altitude | State | AD1,AD2 но они не нужны для правильного разбора координам
        // потому что находятся в конце пакета
        public  byte[] CreatePacket()
        {



            List<byte> Packet = new List<byte>() { 0x24, 0x24/*старт*/, 0x00, 0x4A,/*длина*/ 0x13, 0x61, 0x23, 0x45, 0x67, 0x8f, 0xff,/*идентификатор*/ 0x99, 0x55/*команда*/};

            DateTime dt = DateTime.Now;
            Random random = new Random();
            int number = 0;
            StringBuilder sb = new StringBuilder();
            number = random.Next(0, 2);
            sb.Append(number);
            number = random.Next(0, 3);
            sb.Append(number);
            number = random.Next(0, 5);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);

            number = random.Next(0, 5);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(".");
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(",");
            sb.Append("A");
            sb.Append(",");
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 6);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(".");
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(",");
            sb.Append(random.Next(0, 1000) > 500 ? "N" : "S");
            sb.Append(",");
            number = random.Next(0, 1);
            sb.Append(number);
            number = random.Next(0, 8);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(".");
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(",");
            sb.Append(random.Next(0, 1000) > 500 ? "W" : "E");
            sb.Append(",");
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(".");
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(",");
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(".");
            number = random.Next(0, 9);
            sb.Append(number);
            number = random.Next(0, 9);
            sb.Append(number);
            sb.Append(",");
            sb.Append(dt.ToString("ddMMyy"));

            //sb.

            //String Data = "134829.486,A,1126.6639,S,11133.3299,W,58.31,309.62,110200";// из инструкции
            String Data2 = "134829.486,A,5650.4552,N,06039.0007,E,58.31,309.62,110200";
            String Data = sb.ToString();
            Packet.AddRange(Encoding.ASCII.GetBytes(Data));



            ushort crc = CRCCalculator.Calculate(Packet.ToArray(), Packet.Count);

            Packet.Add(BitConverter.GetBytes(crc)[1]);    // CRC
            Packet.Add(BitConverter.GetBytes(crc)[0]);  // CRC
            Packet.Add(0x0d);  // cr
            Packet.Add(0x0a);  // lf

            List<string> s = new List<string>();

            for (int i = 0; i < Packet.Count; i++)
            {
                s.Add(string.Format("{0:X2}", Packet[i]));
            }

            string pack = string.Join(" ", s.ToArray());

            return Packet.ToArray();

        }
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
        public static ushort Calculate(byte[] data, int dataLength)
        {
            Crc16Ccitt c = new Crc16Ccitt(InitialCrcValue.NonZero1);
            return c.ComputeChecksum(data,dataLength);
        }
    }
}
