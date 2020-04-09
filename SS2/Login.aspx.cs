using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Security;
using System.Configuration;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string ConnectionString = "Data Source=10.1.32.252,1433;Network Library=DBMSSOCN;Initial Catalog=kiosk;User ID=" + txtUsername.Text + ";Password=" + txtPassword.Text;  //  Don't ever do this.

        using (SqlConnection con = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand("", con))
        {
            try
            {
                con.Open();
                con.Close();
                FormsAuthentication.RedirectFromLoginPage("User", chkPersist.Checked);
            }
            catch (Exception)
            {
                lblError.Visible = true;
            }
        }


    }

}