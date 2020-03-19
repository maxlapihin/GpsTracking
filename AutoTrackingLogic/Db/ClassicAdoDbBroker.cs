using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using AutoTrackingLogic.Properties;

namespace AutoTrackingLogic.Db
{
    public class ClassicAdoDbBroker : IDbBroker
    {

        static ClassicAdoDbBroker()
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
                                                   (@DeviceId
                                                   ,@PhoneNumber
                                                   ,@Longitude
                                                   ,@Latitude
                                                   ,@Velocity
                                                   ,@Altitude
                                                   ,@SourceAddress,
                                                    @SourcePort)";


        public void Save(TrackerData data)
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                using (IDbCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandType = CommandType.Text;
                    command.CommandText = _commandText;

                    IDbDataParameter param0 = command.CreateParameter();
                    param0.ParameterName = "@DeviceId";
                    param0.Value = data.DeviceId;

                    IDbDataParameter param1 = command.CreateParameter();
                    param1.ParameterName = "@PhoneNumber";
                    param1.Value = data.PhoneNumber;

                    IDbDataParameter param2 = command.CreateParameter();
                    param2.DbType = DbType.Decimal;
                    param2.ParameterName = "@Longitude";
                    param2.Value = data.Longitude;
                    param2.Precision = 10;
                    param2.Scale = 7;


                    IDbDataParameter param3 = command.CreateParameter();
                    param3.DbType = DbType.Decimal;
                    param3.ParameterName = "@Latitude";
                    param3.Value = data.Latitude;
                    param3.Precision = 10;
                    param3.Scale = 7;


                    IDbDataParameter param4 = command.CreateParameter();
                    param4.DbType = DbType.Decimal;
                    param4.ParameterName = "@Velocity";
                    param4.Value = data.Velocity;
                    param4.Precision = 10;
                    param4.Scale = 7;


                    IDbDataParameter param5 = command.CreateParameter();
                    param5.DbType = DbType.Decimal;
                    param5.ParameterName = "@Altitude";
                    param5.Value = data.Altitude;
                    param5.Precision = 10;
                    param5.Scale = 7;

                    IDbDataParameter param6 = command.CreateParameter();
                    param6.ParameterName = "@SourceAddress";
                    param6.Value = data.SourceAddress;


                    IDbDataParameter param7 = command.CreateParameter();
                    param7.ParameterName = "@SourcePort";
                    param7.Value = data.SourcePort;

                    command.Parameters.Add(param0);
                    command.Parameters.Add(param1);
                    command.Parameters.Add(param2);
                    command.Parameters.Add(param3);
                    command.Parameters.Add(param4);
                    command.Parameters.Add(param5);
                    command.Parameters.Add(param6);
                    command.Parameters.Add(param7);

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
