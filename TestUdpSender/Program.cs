using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;

namespace TestUdpSender
{
    class Program
    {
        static decimal _totalSent = 0;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("94.137.235.141"), 11000);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
            IPacketCreator creator = new EmulatorPacketCreator();
            using (UdpClient udp = new UdpClient())
            {
                while (_totalSent < 99999999)
                {
                    Console.Clear();
                    Console.WriteLine("Send UDP to {0}: total count {1}", endPoint, _totalSent);
                    if (_cancel)
                    {
                        break;
                    }

                    try
                    {
                        byte[] packet = creator.CreatePacket();
                        udp.Send(packet, packet.Length, endPoint);
                        //Thread.Sleep(1);
                        _totalSent++;
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Thread.Sleep(100);
                        return;
                    }

                }
            }
            Console.WriteLine("Exit!");
            Console.ReadLine ();
            Thread.Sleep(1500);
        }

        static bool _cancel;
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _cancel = e.Cancel;
        }
    }
}
