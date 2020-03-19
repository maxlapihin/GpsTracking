using System;
using System.Collections.Generic;
using System.Text;

namespace TestUdpSender
{
    public interface IPacketCreator
    {
        byte[] CreatePacket();
    }
}
