using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Security;

public partial class Controller : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static string LogOff()
    {
        FormsAuthentication.SignOut();
        return "1";
    }


    [WebMethod]
    public static string GetClientIDs(string region)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        Dictionary<String, String> clients = new Dictionary<string, string>();
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT id, kiosk_id FROM kiosk WHERE region=@region ORDER BY id ASC", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("region", region);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                clients[rdr["id"].ToString()] = rdr["kiosk_id"].ToString();
            }
        }
        return serializer.Serialize(clients);
    }

    [WebMethod]
    public static string GetClient(string id)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        Session session = new Session(Convert.ToInt32(id));
        return serializer.Serialize(session);
    }

    [WebMethod]
    public static string GetSelectedRegion(string region)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        List<Session> clients = new List<Session>();
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT * FROM kiosk WHERE region=@region AND registered = 1 ORDER BY id ASC", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("region", region);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                clients.Add(new Session
                {
                    ID = Convert.ToInt32(rdr["id"].ToString()),
                    Kiosk_ID = rdr["kiosk_id"].ToString(),
                    Status = rdr["status"].ToString(),
                    Time = Convert.ToInt32(rdr["time"].ToString()),
                    Login = rdr["login"].ToString(),
                    Password = rdr["password"].ToString(),
                    Renewals = Convert.ToInt32(rdr["renewals"].ToString())

                });
            }
            
        }
        return serializer.Serialize(clients);
    }

    [WebMethod]
    public static string SubmitAutorenew(string region, string property)
    {
        bool autorenew = Convert.ToBoolean(property);
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE autorenew SET autorenew=@autorenew WHERE region=@region", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("autorenew", autorenew);
                cmd.Parameters.AddWithValue("region", region);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return "";
        }
        catch (Exception)
        {
            return "Could not process that request!";
        }
    }

    [WebMethod]
    public static string GetStats(string radioValue, string startDate, string endDate)
    {
        string cmdText = String.Empty;
        string reportType = String.Empty;
        if (radioValue == "total") { cmdText = "SELECT count(*) AS 'Total sessions: ' FROM stats WHERE datetime BETWEEN '" + startDate + "' AND '" + endDate + "'"; reportType = "<h3>Total Session in Date Range</h3>"; }
        if (radioValue == "region") { cmdText = "SELECT count(*) AS Total, kiosk.region FROM stats JOIN kiosk ON (stats.id = kiosk.id) WHERE datetime BETWEEN '" + startDate + "' AND '" + endDate + "' GROUP BY kiosk.region"; reportType = "<h3>Sessions Grouped by Region</h3>";  }
        if (radioValue == "login") { cmdText = "SELECT count(*), type FROM stats WHERE datetime BETWEEN '" + startDate + "' AND '" + endDate + "' GROUP BY type"; reportType = "<h3>Session Grouped by Login Type</h3>"; }


        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
            StringBuilder results = new StringBuilder("<h2>Report for date range</h2>" + reportType);
            if (radioValue != "total") { results.Append("<table>"); }
            int total = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(cmdText, con))
            {
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (radioValue == "total")
                    {
                        results.Append(rdr.GetName(0) + rdr[0].ToString());
                    }
                    else
                    {
                        total += Convert.ToInt32(rdr[0].ToString());
                        results.Append("<tr><td>" + rdr[1].ToString() + ": </td><td>" + rdr[0].ToString() + "</td></tr>");
                    }
                }
                if (radioValue != "total")
                {
                    results.Append("<tr><td>&nbsp;</td><td>&nbsp;</td></tr><tr><td>Total:</td><td>" + total.ToString() + "</td></tr></table>");
                }
                con.Close();
            }
            return results.ToString(); ;
        }
        catch (Exception)
        {
            return "Could not process that request!";
        }
    }


    [WebMethod]
    public static string SendCommand(string command)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE kiosk SET status=@status WHERE registered=1", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("status", command);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return "";
        }
        catch (Exception)
        {
            return "Could not process that request!";
        }
    }

    [WebMethod]
    public static string SendPrinter(string printer)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE kiosk SET selected_printer=@printer", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("printer", printer);
                cmd.ExecuteNonQuery();
                con.Close();
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE printer SET selected_printer=@printer", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("printer", printer);
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return "";
        }
        catch (Exception)
        {
            return "Could not process that request!";
        }
    }

    [WebMethod]
    public static string GetSelectedPrinter()
    {
        try
        {
            string printer = String.Empty;
            string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM printer", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("printer", printer);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    printer = rdr["selected_printer"].ToString();
                }
                con.Close();
            }
            return printer;
        }
        catch (Exception)
        {
            return "Could not process that request!";
        }
    }

    [WebMethod]
    public static string SendMessage(string id, string message)
    {
        Session session = new Session(Convert.ToInt32(id));
        if ((session.Status == "unlocked") || (session.Status == "renewed") || (session.Status == "locked"))
        {
            session.Message = message;
            session.UpdateServer();
            return "";
        }
        else return "Could not send message to the selected client!";
    }

    [WebMethod]
    public static string UpdateAdminClient(string id, string kiosk_id, string region, string mac, string registered, string printer, string password)
    {
        if (password == "1234")  // Don't ever do this.
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Session client = new Session(Convert.ToInt32(id));
                client.Kiosk_ID = kiosk_id;
                client.Region = region;
                client.Mac = mac;
                client.SelectedPrinter = printer;
                client.Registered = Convert.ToBoolean(registered);
                client.UpdateAdminServer();
                return serializer.Serialize(client);
            }
            catch (Exception)
            {
                return "1-error";
            }
        }
        else return "2-error";
    }

    [WebMethod]
    public static string UpdateClient(string status, string id, string time)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        Session session = new Session(Convert.ToInt32(id));
        session.Time = Convert.ToInt32(time);
        session.Status = status;
        if (session.Status == "pending") {
            session.Password = getPassword();
            session.Login = "staff";
            session.Renewals = 0;
        }
        else if (session.Status == "renewed") { session.Renewals++; }
        else { session.EndSession(); }
        session.UpdateServer();
        return serializer.Serialize(session);
    }

    private static string getPassword()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        Random rnd = new Random();
        int id = rnd.Next(1, 9);
        string password = String.Empty;
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT password FROM passwords WHERE id=@id", con))
        {
            cmd.Parameters.AddWithValue("id", id);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                password = rdr["password"].ToString();
            }
        }
        return password;
    }

}