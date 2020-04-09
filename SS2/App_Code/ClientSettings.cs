using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;


class ClientSettings
{
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
    public int MainTimerCount { get; set; }
    public bool AutoRenew { get; set; }
    public int MaxIdleSplash { get; set; }
    public int MaxIdleRestart { get; set; }
    public int Mod { get; set; }


    public ClientSettings()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM settings", con))
            {
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    this.MainTimerCount = Convert.ToInt32(rdr["maintimercount"].ToString());
                    this.Mod = Convert.ToInt32(rdr["mod"].ToString());
                    this.MaxIdleSplash = Convert.ToInt32(rdr["max_idle_splash"].ToString());
                    this.MaxIdleRestart = Convert.ToInt32(rdr["max_idle_restart"].ToString());
                }
                con.Close();
            }
        }
        catch (Exception)
        {
        }
    }

    public void GetAutoRenew(int ID)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT a.autorenew FROM autorenew AS a JOIN kiosk AS k ON (a.region = k.region) WHERE k.id = @id", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("id", ID);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    this.AutoRenew = Convert.ToBoolean(rdr["autorenew"].ToString());
                }
                con.Close();
            }
        }
        catch (Exception)
        {
        }
    }

    public static List<string> GetProcesses()
    {
        List<string> processes = new List<string>();
        return processes;
    }
}

