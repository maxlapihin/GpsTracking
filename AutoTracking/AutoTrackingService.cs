using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using AutoTrackingLogic;

namespace AutoTracking
{
    public partial class AutoTrackingService : ServiceBase
    {
        public AutoTrackingService()
        {
            InitializeComponent();
        }

        private ITransportTracker _trackerService;

        protected ITransportTracker TrackerService
        {
            get {
                if (_trackerService == null)
                {
                    _trackerService = new UdpTransportTracker(); 
                }
                return _trackerService;
            }
        }

        protected override void OnStart(string[] args)
        {
            TrackerService.Start();
        }

        protected override void OnStop()
        {
            TrackerService.Stop ();
        }
    }
}
