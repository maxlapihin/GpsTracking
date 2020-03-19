using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTrackingLogic.Data
{
    public /*struct*/ class Latitude : CoordinateBase
    {
        public override string Str
        {
            get { return base.Str; }
            set
            {
                if (value != "N" && value != "S")
                {
                    throw new ArgumentException("Latitude invalid: " + value);
                }
                base.Str = value;
            }
        }

        protected override byte _MaxDegrees
        {
            get
            {
                return 90;
            }
        }

    }
}
