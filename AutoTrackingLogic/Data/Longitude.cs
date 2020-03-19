using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace AutoTrackingLogic.Data
{
    public /*struct*/ class Longitude : CoordinateBase 
    {
        public override string Str
        {
            get { return base.Str; }
            set
            {
                if (value != "W" && value != "E")
                {
                    value = "W";
                }
                base.Str = value;
            }
        }
    }

}
