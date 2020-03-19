using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTrackingLogic.Db
{
    public interface IDbBroker
    {
        void Save(TrackerData data);
    }
}
