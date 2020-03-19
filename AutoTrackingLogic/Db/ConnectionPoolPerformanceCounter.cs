using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace AutoTrackingLogic.Db
{
    public class ConnectionPoolPerformanceCounter
    {
        private static ConnectionPoolPerformanceCounter _instance;
        public static ConnectionPoolPerformanceCounter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConnectionPoolPerformanceCounter();
                }
                return _instance;
            }
        }

        private const string _category = "Поставщик данных .NET для SqlServer";
        private const string _pooledCounterName = "NumberOfPooledConnections";
        private const string _activeCounterName = "NumberOfActiveConnections";
        private const string _freeCounterName = "NumberOfFreeConnections";

        public ConnectionPoolPerformanceCounter()
        {
            PerformanceCounter.CloseSharedResources();
             
            string instance =AppDomain.CurrentDomain.FriendlyName.Split('.').First() + "[" + Process.GetCurrentProcess().Id + "]" ;
            _pooledCounter = new System.Diagnostics.PerformanceCounter(_category, _pooledCounterName, instance, true);
            _activeCounter = new System.Diagnostics.PerformanceCounter(_category, _activeCounterName, instance, true);
            _freeCounter = new System.Diagnostics.PerformanceCounter(_category, _freeCounterName, instance, true);
        }

        PerformanceCounter _pooledCounter;
        PerformanceCounter _activeCounter;
        PerformanceCounter _freeCounter;

        

        public int PooledConnections
        {
            get
            {
                try
                {
                    return (int)_pooledCounter.NextValue();
                }
                catch (InvalidOperationException)
                {
                    return -1;
                }
            }
        }

        public int ActiveConnections
        {
            get
            {
                try
                {
                    return (int)_activeCounter.NextValue();
                }
                catch (InvalidOperationException)
                {
                    return -1;
                }
            }
        }

        public int FreeConnections
        {
            get
            {
                try
                {
                    return (int)_freeCounter.NextValue();
                }
                catch (InvalidOperationException)
                {
                    return -1;
                }
            }
        }
    }
}

