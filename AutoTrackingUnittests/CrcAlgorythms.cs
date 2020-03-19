using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTrackingLogic.Implementations;

namespace AutoTrackingUnittests
{
    [TestFixture]
    public class CrcAlgorythms
    {
        [Test]
        public void Test1()
        {
            byte[] data = new byte[] { 0x24, 0x24, 0x00, 0x11, 0x13, 0x61, 0x23, 0x45, 0x67, 0x8f, 0xff, 0x50, 0x00 };

            ushort crc = CRCCalculator.Calculate(data, data.Length);


            byte[] crcBytes = BitConverter.GetBytes(crc);

            Assert.AreEqual(2, crcBytes.Length);

            Console.WriteLine("CRC: {0:X2} {1:X2}", crcBytes[0], crcBytes[1]);

            Assert.AreEqual(0x05, crcBytes[1]);
            Assert.AreEqual(0xd8, crcBytes[0]);


        }

        [Test]
        public void Test2()
        {
            byte[] data = new byte[] { 0x24, 0x24, 0x22, 0x11, 0x13, 0x61, 0x23, 0x45, 0x67, 0x8f, 0xff, 0x50, 0x00 };


            ushort crc = CRCCalculator.Calculate(data, data.Length);


            byte[] crcBytes = BitConverter.GetBytes(crc);

            Assert.AreEqual(2, crcBytes.Length);

            Console.WriteLine("CRC: {0:X2} {1:X2}", crcBytes[0], crcBytes[1]);

            Assert.AreEqual(0x0f, crcBytes[1]);
            Assert.AreEqual(0x11, crcBytes[0]);


        }
    }
}
