using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SessionSoftTimer
{
    class LogStats
    {
        public static void Log(int ID, string type)
        {
            string connectionString = "Data Source=SSServer,1433;Network Library=DBMSSOCN;Initial Catalog=kiosk;User ID=yourID;Password=yourPassword;";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO stats (id, type, datetime) VALUES (@id, @type, @datetime)", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("id", ID);
                cmd.Parameters.AddWithValue("type", type);
                cmd.Parameters.AddWithValue("datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    
}
