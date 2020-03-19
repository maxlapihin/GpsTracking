using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.DataAccess;

namespace AutoTrackingLogic.Db
{
    public abstract class TrackerDataAccessor:DataAccessor
    {
        [SprocName("TrackerData_Insert") ]
        public abstract void Save(TrackerData data);
        
    }
}
