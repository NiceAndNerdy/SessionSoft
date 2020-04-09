using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string regionHtml = getRegions();
        regionmenu.InnerHtml = regionHtml;
    }

    private string getRegions()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        StringBuilder builder = new StringBuilder("<li id=\"arrow-left\" class=\"arrow\">&laquo;</li>");
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT region FROM autorenew ORDER BY ID ASC", con))
        {
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                builder.Append("<li class=\"region menu-item\">" + rdr["region"].ToString() + "</li>");
            }
            builder.Append("<li class=\"menu-item\"><a id=\"admin-menu\"  href=\"Admin.aspx\">Admin</a></li>");
        }
        return builder.ToString();
    }
}