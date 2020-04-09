using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SessionSoftTimer
{
    class Session
    {
        private static readonly string connectionString = "Data Source = SSServer,1433; Network Library = DBMSSOCN; Initial Catalog = kiosk; User ID = yourID; Password=yourPassword;";
        public string Status { get; set; }
        public int ID { get; set; }
        public int Time { get; set; }
        public string Message { get; set; }
        public int UntilRestartTime { get; set; }
        public string Password { get; set; }
        public int Renewals { get; set; }
        public string Login { get; set; }
        public SelectedPrinter SelectedPrinter { get; set; }

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
                    this.Status = rdr["status"].ToString();
                    this.ID = Convert.ToInt32(rdr["id"].ToString());
                    this.Time = Convert.ToInt32(rdr["time"].ToString());
                    this.Message = rdr["message"].ToString();
                    this.SelectedPrinter = (SelectedPrinter) Convert.ToInt32(rdr["selected_printer"].ToString());
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
                    this.SelectedPrinter = (SelectedPrinter)Convert.ToInt32(rdr["selected_printer"].ToString());
                }
                con.Close();
            }
        }



        public void UpdateServer()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE kiosk SET status=@status, time=@time, password=@password, renewals=@renewals, login=@login, message=@message WHERE id=@id", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("id", this.ID);
                cmd.Parameters.AddWithValue("status", this.Status);
                cmd.Parameters.AddWithValue("time", this.Time);
                cmd.Parameters.AddWithValue("password", this.Password);
                cmd.Parameters.AddWithValue("renewals", this.Renewals);
                cmd.Parameters.AddWithValue("login", this.Login);
                cmd.Parameters.AddWithValue("message", this.Message);

                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void StartSession(ClientSettings Settings, bool isAdmin, string type, List<String> Processes)
        {
            this.GetSession();
            this.Status = "unlocked";
            this.Time = Settings.MainTimerCount;
            UpdateServer();
            foreach (Process proc in Process.GetProcessesByName("IEXPLORE"))
            {
                proc.Kill();
            }
           
            if (!isAdmin) { KillProcesses(Processes); }
            LogStats.Log(this.ID, type);
        }

        public void ResumeSession()
        {
            if (this.Renewals > 0) { this.Status = "renewed"; } else { this.Status = "unlocked"; } 
            this.UntilRestartTime = 0;
            UpdateServer();
            foreach (Process proc in Process.GetProcessesByName("IEXPLORE"))
            {
                proc.Kill();
            }
        }

        private void KillProcesses(List<String> Processes)
        {
            foreach (string process in Processes)
            {
                foreach (Process proc in Process.GetProcessesByName(process.ToUpper()))
                {
                    proc.Kill();
                }
            }
        }

        public void RenewSession(ClientSettings Settings)
        {
            this.Status = "renewed";
            this.Time = Settings.MainTimerCount;
            this.Renewals++;
            UpdateServer();
            LogStats.Log(this.ID, "clientrenew");
        }

        public void GoIdle()
        {
            this.Status = "idle";
            UpdateServer();
        }

        public void EndSession(ClientSettings Settings)
        {
            this.Status = "locked";
            this.Time = Settings.MainTimerCount;
            this.Message = "";
            this.Password = "none";
            this.Renewals = 0;
            this.Login = "none";
            this.UntilRestartTime = 0;
            UpdateServer();
        }       
    }

}
