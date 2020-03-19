using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTrackingLogic
{
    interface ITrackerDataParser
    {
        TrackerData Parse(byte[] data);
    }
}
