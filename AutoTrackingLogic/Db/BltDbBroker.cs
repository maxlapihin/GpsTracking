using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.Data;
using BLToolkit.Data.Sql;
using BLToolkit.DataAccess;
using System.Data.SqlClient;
using System.Timers;
using AutoTrackingLogic.Properties;

namespace AutoTrackingLogic.Db
{
    public class BltDbBroker:IDbBroker 
    {
        static BltDbBroker()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = @"(local)";
            sb.InitialCatalog = "TrackerDb";
            sb.UserID = "sa";
            sb.Password = "GfhjkmcfGfhjkmcf";
            sb.ConnectTimeout = 60;
            sb.LoadBalanceTimeout = 5;
            sb.MaxPoolSize = (int)(Settings.Default.ThreadsCount * 1.25);
            sb.MinPoolSize = Settings.Default.ThreadsCount;

            DbManager.AddConnectionString(sb.ConnectionString); 
            //DbManager.AddConnectionString(_connectionString);


            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Interval = 2 * 1000;
            _timer.Start(); 
        }

        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SqlConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static Timer _timer;


        #region IDbBroker Members


        public void Save(TrackerData data)
        {
            lock (_timer)
            {
                TrackerDataAccessor.CreateInstance<TrackerDataAccessor>().Save(data);
                //using (DbManager db = new DbManager())
                //{
                //    SqlQuery<TrackerData> query = new SqlQuery<TrackerData>(db);
                //    query.Insert(data);
                //}
            }
        }

        #endregion
    }
}
