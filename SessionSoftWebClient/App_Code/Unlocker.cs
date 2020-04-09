using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Unlocker
/// </summary>
public class Unlocker
{
    public Unlocker()
    {
        //
        // TODO: Add constructor logic here
        //

    }

    public static void Unlock(int ID, string barcode, string type)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("UPDATE kiosk SET status=@status, login=@barcode WHERE id=@id", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("id", ID);
            cmd.Parameters.AddWithValue("status", type);
            cmd.Parameters.AddWithValue("barcode", barcode);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}

