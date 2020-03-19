using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace AutoTrackingLogic.Data
{
    public class CoordinateBase:IFormattable 
    {
        private byte _degrees;
        private byte _minutes;
        private byte _seconds;

        protected virtual byte _MaxDegrees
        {
            get {return 180; }
        }


        public byte Minutes
        {
            get { return _minutes; }
            set
            {
                if (Degrees == _MaxDegrees)
                {
                    value = 0;
                }
                if (value >= 60)
                {
                    Degrees++; 
                    value = 0;
                }
                _minutes = value;
            }
        }

        public byte Seconds
        {
            get { return _seconds; }
            set
            {
                if (Degrees == _MaxDegrees)
                {
                    value = 0;
                }
                else if (value >= 60)
                {
                    Minutes++;
                    value = 0;
                }
                _seconds = value;
            }
        }

        private float _minutesDecimal;
        public float MinutesDecimal
        {
            get { return _minutesDecimal; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 1)
                {
                    value = 1;
                }
                _minutesDecimal = value;
                Seconds =(byte)( _minutesDecimal * 60f);
            }
        }


        public virtual byte Degrees
        {
            get { return _degrees; }
            set
            {
                if (value > _MaxDegrees)
                {
                    value = _MaxDegrees;
                }
                _degrees = value;
            }
        }

        private string _str;

        public virtual string Str
        {
            get { return _str; }
            set
            { _str = value; }
        }

        public double DoubleRepresentation
        {
            get
            {
                return Degrees + Minutes / 60d + Seconds / 3600d;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}-{2}-{3:F4}", Str, Degrees, Minutes, Seconds);
        }


        #region IFormattable Members

        private static string GetStringToReplace(string format, string symbol)
        {
            int indexOfSymbol = format.IndexOf(symbol);
            int lastIndexOfSymbol = format.LastIndexOf(symbol);
            string toReplace = string.Empty;
            if (indexOfSymbol > 0 && lastIndexOfSymbol >= indexOfSymbol)
            {
                toReplace = format.Substring(indexOfSymbol, lastIndexOfSymbol - indexOfSymbol + 1);
            }
            return toReplace;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            //Lyyymmssdddd
            int indexOfL = format.IndexOf("L", 0, format.Length);


            string yToReplace = GetStringToReplace(format,"d");
            string mToReplace = GetStringToReplace(format, "m");
            string sToReplace = GetStringToReplace(format, "s");


            string output = format.Replace("L", Str);
            if (yToReplace != string.Empty)
            {
                output = output.Replace(yToReplace, Degrees.ToString());
            }
            if (mToReplace != string.Empty)
            {
                output = output.Replace(mToReplace, Minutes.ToString());
            }
            if (sToReplace != string.Empty)
            {
                output = output.Replace(sToReplace, Seconds.ToString());
            }


            return output;
            //throw new NotImplementedException();

        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }



        #endregion
    }
}
