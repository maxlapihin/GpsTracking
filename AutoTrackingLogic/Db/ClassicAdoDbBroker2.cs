using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using AutoTrackingLogic.Properties;
using System.Globalization;

namespace AutoTrackingLogic.Db
{
    public class ClassicAdoDbBroker2 : IDbBroker
    {

        static ClassicAdoDbBroker2()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = @"(local)";
            sb.InitialCatalog = "TrackerDb";
            sb.UserID  = "sa";
            sb.Password  = "GfhjkmcfGfhjkmcf";
            sb.ConnectTimeout = 60;
            sb.LoadBalanceTimeout = 5;
            sb.MaxPoolSize = (int)(Settings.Default.ThreadsCount*1.25);
            sb.MinPoolSize = Settings.Default.ThreadsCount;


            _connectionString = sb.ToString();

        }


        private static string _connectionString = string.Empty;


        private static string _commandText = @"INSERT INTO [dbo].[TrackerData]
                                                   ([DeviceId]
                                                   ,[PhoneNumber]
                                                   ,[Longitude]
                                                   ,[Latitude]
                                                   ,[Velocity]
                                                   ,[Altitude]
                                                   ,[SourceAddress]
                                                   ,[SourcePort])
                                             VALUES
                                                   ({0}
                                                   ,'{1}'
                                                   ,{2:F5}
                                                   ,{3:F5}
                                                   ,{4:F5}
                                                   ,{5:F5}
                                                   ,'{6}'
                                                   ,{7})";


        public void Save(TrackerData data)
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                using (IDbCommand command = new SqlCommand())
                {
                    

                    command.Connection = conn;
                    command.CommandType = CommandType.Text;

                    NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

                    command.CommandText = string.Format(nfi, _commandText,
                        data.DeviceId, data.PhoneNumber, data.Longitude,
                        data.Latitude, data.Velocity, data.Altitude,
                        data.SourceAddress, data.SourcePort);

                    Console.WriteLine(command.CommandText);
                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}
