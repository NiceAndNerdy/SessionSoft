using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using SIP2;
using System.Net;
using System.Xml;
using System.Configuration;

public partial class Controller : System.Web.UI.Page
{
    [WebMethod]
    public static string RegisterKiosk(string mac, string region, string kiosk_id, string password)
    {

        bool hasMAC = GetMAC(mac);
        if (hasMAC) { return "mac"; }

        try
        {
            if (password != "1234")
            {
                return "x";
            }
            else
            {
                string id = String.Empty;
                string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("UPDATE kiosk SET registered = 1, mac=@mac WHERE region=@region AND kiosk_id=@kiosk_id", con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("mac", mac);
                    cmd.Parameters.AddWithValue("region", region);
                    cmd.Parameters.AddWithValue("kiosk_id", kiosk_id);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return GetID(mac);
            }
        }
        catch (Exception)
        {
            return "x";
        }
    }

    private static bool GetMAC(string mac)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT id FROM kiosk WHERE mac=@mac", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("mac", mac);
            SqlDataReader rdr = cmd.ExecuteReader();
            return rdr.HasRows; 
            con.Close();
        }
    }



    private static string GetID(string mac)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        string id = String.Empty;
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT id FROM kiosk WHERE mac=@mac", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("mac", mac);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                id = rdr["id"].ToString();
            }
            con.Close();
        }
        return id; 
    }

    [WebMethod]
    public static string GetAvailableKiosks()
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        Dictionary<String, List<String>> availableKiosks = new Dictionary<string, List<string>>();
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT kiosk_id, region FROM kiosk WHERE registered=0", con))
        {
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (availableKiosks.ContainsKey(rdr["region"].ToString()))
                {
                    availableKiosks[rdr["region"].ToString()].Add(rdr["kiosk_id"].ToString());
                }
                else
                {
                    availableKiosks[rdr["region"].ToString()] = new List<string> { rdr["kiosk_id"].ToString() };
                }
            }
            con.Close();
        }
        return serializer.Serialize(availableKiosks);
    }



    [WebMethod]
    public static string LoginKoha(string username, string pin, string id)
    {
	int intID = Convert.ToInt32(id);
	if (intID > 50) return "This option has been disabled by the administrator.  Please see a librarian for help logging in.";
	WebClient client = new WebClient();
        string downloadString = client.DownloadString("https://yourKohaServer/cgi-bin/koha/ilsdi.pl?service=AuthenticatePatron&username=" + username + "&password=" + pin);
        XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
        xmlDoc.LoadXml(downloadString);
        XmlNodeList patronid = xmlDoc.GetElementsByTagName("id");
        if (patronid[0] != null)
        {
            string patronID = patronid[0].InnerText;
            downloadString = client.DownloadString("https://yourKohaServer/cgi-bin/koha/ilsdi.pl?service=GetPatronInfo&patron_id=" + patronID);
            xmlDoc.LoadXml(downloadString);
            XmlNodeList cardnumber = xmlDoc.GetElementsByTagName("cardnumber");
            string patronCardnumber = cardnumber[0].InnerText;
            Unlocker.Unlock(Convert.ToInt32(id), patronCardnumber, "startkoha");
            if (CheckSip(patronCardnumber))
            {
                return true.ToString();
            }
            else return "There seems to be a problem with this account.  Please see a librarian.";
        }
        else return "Invalid username and/or pin!";
    }

    [WebMethod]
    public static string LoginSIP(string barcode, string id)
    {
        int intID = Convert.ToInt32(id);
	if (CheckSip(barcode))
        {
            Unlocker.Unlock(Convert.ToInt32(id), barcode, "startsip");
            return true.ToString();
        }
        else return "There seems to be a problem with this account.  Please see a librarian.";
    }

    private static bool CheckSip(string barcode)
    {
        SipConnection sip = new SipConnection("SIPIP", "SIPPORT", "SIPUSER", "SIPPASSWORD");
        sip.Open();
        bool authorized = sip.AuthorizeBarcode(barcode);
        sip.Close();
        return authorized;
    }

    [WebMethod]
    public static string LoginLocal(string password, string id)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        string serverPassword = String.Empty;
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT password FROM kiosk WHERE id=@id AND status='pending'", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("id", id);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                serverPassword = rdr["password"].ToString();
            }
            con.Close();
        }

        if (password == serverPassword)  { Unlocker.Unlock(Convert.ToInt32(id), "Staff", "startstaff"); return true.ToString(); }
        else return "Invalid password!";
    }

}