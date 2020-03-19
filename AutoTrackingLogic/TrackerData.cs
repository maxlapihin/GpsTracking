using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTrackingLogic
{
    public /*struct*/class TrackerData
    {
        public int DeviceId { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        ///Широта
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        ///Долгота
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        ///Скорость
        /// </summary>
        public double Velocity { get; set; }

        /// <summary>
        ///Высота
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        ///ДатаВремя
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        ///Источник - адрес
        /// </summary>
        public string SourceAddress { get; set; }

        /// <summary>
        ///Источник - порт
        /// </summary>
        public int  SourcePort { get; set; }


        public static TrackerData Empty = new TrackerData() { Altitude = double.NaN, DateTime = DateTime.MinValue, Latitude = 0, Longitude = 0, PhoneNumber = string.Empty, Velocity = double.NaN };
    
    }

}
