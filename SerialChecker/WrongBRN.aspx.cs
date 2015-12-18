using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SerialChecker
{
    public partial class WrongBRN : System.Web.UI.Page
    {
        SqlConnection advanceConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["advance"].ConnectionString);
        static string fullname = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string user = Page.User.Identity.Name;
            DirectorySearcher dirSearcher = new DirectorySearcher();
            DirectoryEntry entry = new DirectoryEntry(dirSearcher.SearchRoot.Path);
            dirSearcher.Filter = "(&(objectClass=user)(objectcategory=person)(samaccountname=" + user.Replace(@"RSP\", "") + "*))";
            SearchResult srfullName = dirSearcher.FindOne();
            string propName = "displayname";
            ResultPropertyValueCollection valColl = srfullName.Properties[propName];
            fullname = Convert.ToString(valColl[0]);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string custNo = Request.QueryString["custNo"];
            string name = Request.QueryString["name"];
            string brn = Request.QueryString["brn"];
            if (txtBrn.Text.Length > 0)
            {
                SqlCommand insertCmd = new SqlCommand("insert into scwrongbrn ([custNo], [name], [wrongbrn], [correctbrn], [insertedby], [inserteddatetime]) " +
                    "values(@custNo, @name, @wrongbrn, @correctbrn, @insertedBy, @insertedDatetime)", advanceConn);
                insertCmd.Parameters.AddWithValue("@custNo", custNo);
                insertCmd.Parameters.AddWithValue("@name", name);
                insertCmd.Parameters.AddWithValue("@wrongbrn", brn);
                insertCmd.Parameters.AddWithValue("@correctbrn", txtBrn.Text);
                insertCmd.Parameters.AddWithValue("@insertedBy", fullname);
                insertCmd.Parameters.AddWithValue("@insertedDatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                advanceConn.Open();
                insertCmd.ExecuteNonQuery();
                advanceConn.Close();

                divMsg.Attributes.Add("class", "show alert alert-success alert-dismissible");
            }
            else
            {
                divBrn.Attributes.Add("class", "form-group has-error has-feedback");
            }
        }
    }
}