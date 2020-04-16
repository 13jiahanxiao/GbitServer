using System;
using MySql.Data.MySqlClient;

namespace GameServer.Tool
{
    class MySqlConnectTool
    {
        public const string CONNECTSTRING = "database=gbitgame;data source=127.0.0.1;port=3306;user=root;pwd=763980";

        public static MySqlConnection Connect()
        {
            MySqlConnection myConnect = new MySqlConnection(CONNECTSTRING);
            try
            {
                myConnect.Open();
                return myConnect;
            }
            catch (Exception e)
            {
                Console.WriteLine("database connect wrong" + e);
                return null;
            }
        }

        public static void CloseConnect(MySqlConnection myCon)
        {
            if (myCon != null)
            {
                myCon.Close();
            }
            else Console.WriteLine("MySqlConnect is null can't close");
        }
    }
}
