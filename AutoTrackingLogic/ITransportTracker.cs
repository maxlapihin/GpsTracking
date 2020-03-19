using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTrackingLogic
{
    public interface ITransportTracker
    {
        void Start();
        void Stop();
        TrackerData Receive(byte[] data);
    }
}
