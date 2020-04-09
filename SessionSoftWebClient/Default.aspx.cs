using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string id = Request.QueryString["id"];
        clientInfo client = new clientInfo();
        if (!String.IsNullOrEmpty(id))
        {
             client = getClientInfo(id);
        }
        else { client.region = ""; }

        if (client.region == "")
        {
            main.InnerHtml = "<p id=\"registration-container\">It appears that this machine is not registered.  Would you like to add it to the database?<br /><br /><input type=\"submit\" value=\"Register\" id=\"registration\" /></p>"; 
        }
        else
        {
            tag.InnerHtml = "<span id=\"tag-info\">" + client.region.ToUpper() + " " + client.ID + "</span>";
        }
    }

    private clientInfo getClientInfo(string id)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT kiosk_id, region FROM kiosk WHERE id=@id", con))
        {
            cmd.Parameters.AddWithValue("id", id);
            con.Open();

            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                clientInfo client = null;
                while (rdr.Read())
                {
                    client = new clientInfo { region = rdr["region"].ToString(), ID = rdr["kiosk_id"].ToString() };
                }
                con.Close();
                return client;
            }
            else
            {
                con.Close();
                return null;
            }
        }

    }

}

class client
{
    public string region { get; set; }
    public string ID { get; set; }
}


