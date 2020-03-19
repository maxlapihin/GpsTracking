using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTrackingLogic;

namespace AutoTrackingConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Console.WriteLine("Starting AutoTracking Service...");
            ITransportTracker tracker = null;
            try
            {
                tracker = new UdpTransportTracker();
                //tracker = new AutographTransportTracker();
                tracker.Start();
                Console.WriteLine("AutoTracking Service started!");
                Console.ReadLine();  
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error!!!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
            finally
            {
                if (tracker != null)
                {
                    tracker.Stop();    
                }
                Console.WriteLine("AutoTracking Service stopped!");
            }
            Console.ReadLine();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //e.IsTerminating = false;
            Console.WriteLine(e.ExceptionObject);
        }
    }
}
