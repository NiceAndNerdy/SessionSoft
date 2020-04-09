using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for Session
/// </summary>
public class Session
{
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
    public string Status { get; set; }
    public int ID { get; set; }
    public int Time { get; set; }
    public string Message { get; set; }
    public int UntilRestartTime { get; set; }
    public string Password { get; set; }
    public int Renewals { get; set; }
    public string Login { get; set; }
    public string Kiosk_ID { get; set; }
    public bool Registered { get; set; }
    public string Mac { get; set; }
    public string Region { get; set; }
    public string SelectedPrinter { get; set; }

    public Session()
    {
    }


    public Session(int ID)
    {
        GetSession(ID);
    }

    public Session(string Mac)
    {
        this.ID = 0;
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM kiosk WHERE mac=@mac", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("mac", Mac);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    this.Status = rdr["status"].ToString();
                    this.ID = Convert.ToInt32(rdr["id"].ToString());
                    this.Time = Convert.ToInt32(rdr["time"].ToString());
                    this.Message = rdr["message"].ToString();
                    this.Password = rdr["password"].ToString();
                    this.Renewals = Convert.ToInt32(rdr["renewals"].ToString());
                    this.Login = rdr["login"].ToString();
                    this.SelectedPrinter = rdr["selected_printer"].ToString();
                }
                con.Close();
            }
        }
        catch (Exception ex)
        {
            string error = ex.Message;
            this.ID = -1;
        }
    }

    private void GetSession(int ID)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT * FROM kiosk WHERE id=@id", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("id", ID);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                this.ID = Convert.ToInt32(rdr["id"].ToString());
                this.Status = rdr["status"].ToString();
                this.Time = Convert.ToInt32(rdr["time"].ToString());
                this.Password = rdr["password"].ToString();
                this.Message = rdr["message"].ToString();
                this.Login = rdr["login"].ToString();
                this.Renewals = Convert.ToInt32(rdr["renewals"].ToString());
                this.Kiosk_ID = rdr["kiosk_id"].ToString();
                this.Registered = Convert.ToBoolean(rdr["registered"].ToString());
                this.Mac = rdr["mac"].ToString();
                this.Region = rdr["region"].ToString();
                this.SelectedPrinter = rdr["selected_printer"].ToString();
            }
            con.Close();
        }
    }



    public void GetSession()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT * FROM kiosk WHERE id=@id", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("id", this.ID);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                this.ID = Convert.ToInt32(rdr["id"].ToString());
                this.Status = rdr["status"].ToString();
                this.Time = Convert.ToInt32(rdr["time"].ToString());
                this.Password = rdr["password"].ToString();
                this.Message = rdr["message"].ToString();
                this.Login = rdr["login"].ToString();
                this.Renewals = Convert.ToInt32(rdr["renewals"].ToString());
                this.Kiosk_ID = rdr["kiosk_id"].ToString();
                this.Registered = Convert.ToBoolean(rdr["registered"].ToString());
                this.Mac = rdr["mac"].ToString();
                this.Region = rdr["region"].ToString();
                this.SelectedPrinter = rdr["selected_printer"].ToString();
            }
            con.Close();
        }
    }

    public void UpdateAdminServer()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("UPDATE kiosk SET mac=@mac, region=@region, kiosk_id=@kiosk_id, registered=@registered, selected_printer=@selected_printer WHERE id=@id", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("id", this.ID);
            cmd.Parameters.AddWithValue("mac", this.Mac);
            cmd.Parameters.AddWithValue("region", this.Region);
            cmd.Parameters.AddWithValue("kiosk_id", this.Kiosk_ID);
            cmd.Parameters.AddWithValue("registered", this.Registered);
            cmd.Parameters.AddWithValue("selected_printer", this.SelectedPrinter);

            cmd.ExecuteNonQuery();
            con.Close();
        }
    }

    public void UpdateServer()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("UPDATE kiosk SET status=@status, time=@time, password=@password, renewals=@renewals, login=@login, message=@message, selected_printer=@selected_printer WHERE id=@id", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("id", this.ID);
            cmd.Parameters.AddWithValue("status", this.Status);
            cmd.Parameters.AddWithValue("time", this.Time);
            cmd.Parameters.AddWithValue("password", this.Password);
            cmd.Parameters.AddWithValue("renewals", this.Renewals);
            cmd.Parameters.AddWithValue("login", this.Login);
            cmd.Parameters.AddWithValue("message", this.Message);
            cmd.Parameters.AddWithValue("selected_printer", this.SelectedPrinter);

            cmd.ExecuteNonQuery();
            con.Close();
        }
    }

    public void EndSession()
    {
        ClientSettings Settings = new ClientSettings();
        this.Time = Settings.MainTimerCount;
        this.Message = "";
        this.Password = "none";
        this.Renewals = 0;
        this.Login = "none";
    }
}
