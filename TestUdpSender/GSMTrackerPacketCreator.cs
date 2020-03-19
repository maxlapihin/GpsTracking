using System;
using System.Collections.Generic;
using System.Text;

namespace TestUdpSender
{
    public class GSMTrackerPacketCreator:IPacketCreator
    {

        public byte[] CreatePacket()
        {
            string packet = @"IMEI 355358047054484
$GPRMC,195501.000,A,5659.09567,N,06033.36329,E,2.0,247.8,150111,14.3,E,A*3D
$GPGGA,195458.000,5659.09530,N,06033.36159,E,1,05,2.3,313.6,M,-8.8,M,,*48
CurCell 13224229 LAC 36601 Name MegaFon MCC 250 MNC 02 MODE 6 SSI 95
*AA2288E7";

            return Encoding.ASCII.GetBytes(packet);
        }
    }
}
