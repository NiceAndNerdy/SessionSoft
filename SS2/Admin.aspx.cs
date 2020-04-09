using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

public partial class Admin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Selects pageSelects = getRegions();
        regionmenu.InnerHtml = pageSelects.menu;
        autorenewContainer.InnerHtml = pageSelects.autoRenew; 
        manageSelect.InnerHtml = pageSelects.manageSelects;
        subManageSelect.InnerHtml = pageSelects.subManageSelects;
        messageSelect.InnerHtml = pageSelects.messageSelects;   
    }

    private Selects getRegions()
    {
        Selects pageSelects = new Selects();
        string connectionString = ConfigurationManager.ConnectionStrings["KioskConnectionString"].ConnectionString;
        StringBuilder builder = new StringBuilder("<li id=\"arrow-left\" class=\"arrow\">&laquo;</li>");
        StringBuilder autorenewBuilder = new StringBuilder("<h2 style=\"margin-bottom:10px;\">Autorenew Settings</h2><table id=\"autorenew-table\">");
        StringBuilder managebuilder = new StringBuilder("<select class=\"manage-select\">");
        StringBuilder subManagebuilder = new StringBuilder("<select id=\"client-region\">");
        StringBuilder messagebuilder = new StringBuilder("<select class=\"manage-select\">");
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT * FROM autorenew ORDER BY ID ASC", con))
        {
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                builder.Append("<li class=\"region menu-item\"><a href=\"Default.aspx?region=" + rdr["region"].ToString() + "\">" + rdr["region"].ToString() + "</a></li>");
                string boxValue = (Convert.ToBoolean(rdr["autorenew"].ToString())) ? "checked" : "";
                autorenewBuilder.Append("<tr><td>" + rdr["region"].ToString() + "</td><td><input type=\"checkbox\" class=\"autocheck\" id=\"" + rdr["region"].ToString() + "-checkbox\" " + boxValue + " data-region=\"" + rdr["region"].ToString() + "\" /></td></tr>");
                managebuilder.Append("<option value=\"" + rdr["region"].ToString() + "\">" + rdr["region"].ToString() + "</option>");
                subManagebuilder.Append("<option value=\"" + rdr["region"].ToString() + "\">" + rdr["region"].ToString() + "</option>");
                messagebuilder.Append("<option value=\"" + rdr["region"].ToString() + "\">" + rdr["region"].ToString() + "</option>");
            }
            builder.Append("<li class=\"menu-item\"><a id=\"admin-menu\"  href=\"Admin.aspx\">Admin</a></li>");
            autorenewBuilder.Append("</table><input type=\"submit\" value=\"Cancel\" class=\"cancel\" />");
            managebuilder.Append("</select>");
            subManagebuilder.Append("</select>");
            messagebuilder.Append("</select>");
            pageSelects.menu = builder.ToString();
            pageSelects.autoRenew = autorenewBuilder.ToString();
            pageSelects.manageSelects = managebuilder.ToString();
            pageSelects.subManageSelects = subManagebuilder.ToString();
            pageSelects.messageSelects = messagebuilder.ToString();

        }
        return pageSelects;
    }
    
}


struct Selects
{
    public string menu { get; set; }
    public string autoRenew { get; set; }
    public string manageSelects { get; set; }
    public string subManageSelects { get; set; }
    public string messageSelects { get; set; }
    
}