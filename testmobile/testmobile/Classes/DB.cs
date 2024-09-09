
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xamarin.Forms;

namespace testmobile
{
    public class DB : IDisposable
    {
                
        MySqlConnection conn = new MySqlConnection("Server = db4free.net; Port = 3306; Database = mobile_test; Uid = mobile; Pwd = mobile12; Allow User Variables=True;");

       

        public void openConn()
        {
            if(conn.State == System.Data.ConnectionState.Closed)
                conn.Open();
        }
        public void closeConn()
        {
            if (conn.State == System.Data.ConnectionState.Open)
                conn.Close();
        }

        public int get_user_id(string login)
        {
            int id;
            using (DB dB = new DB())
            {
                dB.openConn();
                using (MySqlCommand cmd = new MySqlCommand("SELECT id FROM `Users` WHERE `login` = @uL", dB.getConn()))
                {
                    cmd.Parameters.Add("@uL", MySqlDbType.VarChar).Value = login;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id"));
                            dB.closeConn();
                            return id;                         
                        }
                        else
                        {
                            dB.closeConn();
                            return 0;
                        }
                    }
                } 
                
            }
            
                
                
            
           
            
        }
        public MySqlConnection getConn()
        {
            return  conn; 
        }
        
         public void Dispose()
    {
        conn.Dispose();
    }
    }
}
