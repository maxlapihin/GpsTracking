using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using AutoTrackingLogic.Properties;
using AutoTrackingLogic.Implementations;
using AutoTrackingLogic.Db;

namespace AutoTrackingLogic
{
    public class UdpTransportTracker:ITransportTracker 
    {

        private Thread _listenerThread;
        private static IList<ITrackerDataParser>  _parsers;
        delegate void ResponseHandler(IPEndPoint RemoteIp, byte[] Datagram);

        public UdpTransportTracker()
        {
        }
        static UdpTransportTracker()
        {
            _parsers = new List<ITrackerDataParser>();
            ThreadPool.SetMaxThreads(MaxThreadsCount, 1000);
        }

        private static int MaxThreadsCount
        {
            get
            {
                return Settings.Default.ThreadsCount;
            }
        }

        static object _syncRoot = new object(); 
        bool _running;

        public bool Running
        {
            get
            {
                lock (_syncRoot)
                {
                    return _running;
                }
            }
            protected set
            {
                lock (_syncRoot)
                {
                    _running = value;
                }
            }
        }
#if DEBUG
        DateTime _last = DateTime.Now;
#endif
        private void UdpListener()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            ResponseHandler response = new ResponseHandler(UdpResponseHandler);
            WaitCallback callback = new WaitCallback(UserWorkItemHandler);
            object[] args = new object[2];
            byte[] result = null;
            bool queuedSuccessfully = false;
            using (UdpClient udp = new UdpClient(Settings.Default.UdpPort))
            {
                while (Running)
                {
                    try
                    {
                        result = udp.Receive(ref endPoint);
#if DEBUG
                        if ((DateTime.Now - _last).TotalSeconds  > 2)
                        {
                            _last = DateTime.Now;
                            //Console.Clear();
                            //int workerThreads, completionWorkerThreads;
                            //ThreadPool.GetAvailableThreads(out workerThreads, out completionWorkerThreads);
                            //string info = string.Format("Threads in pool available: Workers={0}. Asynchronous I/O={1}", workerThreads, completionWorkerThreads);

                            //string info2 = string.Format("Connections: Pooled:{0}, Free:{1} ",
                            //    ConnectionPoolPerformanceCounter.Instance.PooledConnections,
                            //    ConnectionPoolPerformanceCounter.Instance.FreeConnections
                            //    );

                            //Console.WriteLine(info);
                            //Console.WriteLine(info2);
                        }
#endif
                        //while (ConnectionPoolPerformanceCounter.Instance.FreeConnections == 0)
                        //{
                        //    Console.WriteLine("[{0}]     No free connections in pool. Wait...",DateTime.Now.ToString()   );
                        //    Thread.Sleep(100);
                        //}

                        args[0] = endPoint;
                        args[1] = result;
                        //ThreadPool.QueueUserWorkItem(callback, args);
                        do
                        {
                            queuedSuccessfully = ThreadPool.UnsafeQueueUserWorkItem(callback, args);
                            if (!queuedSuccessfully) Console.WriteLine("[{0}]     UPD Handling is not queued successfully. Thread pool is bizy.", DateTime.Now.ToString());
                        }
                        while (!queuedSuccessfully);
                        
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Thread.Sleep(100);
                    }
                }
            }

        }

        private void UserWorkItemHandler(object state)
        {
            object[] args = (object[])state;
            IPEndPoint RemoteIp = (IPEndPoint)args[0];
            byte[] Datagram = (byte[])args[1];

            UdpResponseHandler(RemoteIp, Datagram);
        }
       
        private void UdpResponseHandler(IPEndPoint RemoteIp, byte[] Datagram)
        {
#if DEBUG
            //string info = string.Format("UDP datagram received:Host={0}. Length={1}",RemoteIp.ToString(), Datagram.Length);
            //Console.WriteLine(info);
#endif
            TrackerData data;
            try
            {
                data = Receive(Datagram);
                //#if DEBUG
                data.SourceAddress = RemoteIp.Address.ToString();
                data.SourcePort = RemoteIp.Port;
                //#endif
                Save(data);

            }
            catch (TrackerDataParserException ex)
            {
                //отбрасываем пакет
                Console.WriteLine(ex);
            }
            catch (NotImplementedException ex)
            {
                //отбрасываем пакет
                Console.WriteLine(ex);
            }
        }

        private void Save(TrackerData data)
        {
            //do save database here!
            DbBroker.Instance.Save(data);
        }

        
        #region ITransportTracker Members


        public void Start()
        {
            _listenerThread = new Thread(new ThreadStart(UdpListener));
            Running = true;
            _listenerThread.Start();
        }

        public void Stop()
        {
            Running = false;
            while (_listenerThread.ThreadState != ThreadState.Stopped)
            {
                _listenerThread.Join(1000); 
            }
        }   


        public TrackerData Receive(byte[] data)
        {
            //здесь логика выбора парсера в зависимости от типа пакета (контроллера)
            ITrackerDataParser parser = GetParserByData(data);
            if (parser == null)
            {
                throw new NotImplementedException("Парсер для данного пакета не реализован");
            }

#if DEBUG
            //string info = string.Format("Parser:{0}", parser.GetType().Name);
            //Console.WriteLine(info);
#endif
            try
            {
                return parser.Parse(data); 
            }
            catch (Exception e)
            {
                throw new TrackerDataParserException("Cannot parse packet. See inner exception for details", e);
            }
           
        }


        private ITrackerDataParser GetParserByData(byte[] data)
        {
            Console.WriteLine(Encoding.ASCII.GetString(data));
            if (Encoding.ASCII.GetString(data).StartsWith("EMULATOR"))
            {
                return GetParserInternal<EmulatorTrackerDataParser>();
            }
            if (Encoding.ASCII.GetString(data).Contains("$GPRMC"))
            {
                return GetParserInternal<NMEA0183DataParser>();
            }
            else if ((data[0] == 0x24) && (data[1] == 0x24) && (data[data.Length - 2] == 0x0d) && (data[data.Length - 1] == 0x0a))
            {
                return GetParserInternal<MeiligaoDataParser>();
            }
            else
            {
                
                return GetParserInternal<TestTrackerDataParser>();  
            }
        }

        private static object _parsersLock = new object();
        private ITrackerDataParser GetParserInternal<T>() where T : ITrackerDataParser, new()
        {
            lock (_parsersLock)
            {
                if (_parsers.OfType<T>().FirstOrDefault() == null)
                {
                    _parsers.Add(new T());
                }
                return _parsers.OfType<T>().FirstOrDefault();
            }
        }
        #endregion
    }
}
