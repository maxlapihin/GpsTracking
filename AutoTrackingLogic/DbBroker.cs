using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using AutoTrackingLogic.Db;

namespace AutoTrackingLogic
{
    public class DbBroker 
    {

        private DbBroker()
        {

        }

        private static IDbBroker _instance;

        public static IDbBroker Instance
        {
          get {
              if (_instance == null)
              {
                  //_instance = new BltDbBroker(); 
                  _instance = new ClassicAdoDbBroker2();
              }
              return _instance ;
          }
        }
    }
}
