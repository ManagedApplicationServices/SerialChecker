using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;

namespace SerialChecker
{
    public partial class SiteMaster : MasterPage
    {
        public string fullname = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                string user = Page.User.Identity.Name;
                DirectorySearcher dirSearcher = new DirectorySearcher();
                DirectoryEntry entry = new DirectoryEntry(dirSearcher.SearchRoot.Path);
                dirSearcher.Filter = "(&(objectClass=user)(objectcategory=person)(samaccountname=" + user.Replace(@"RSP\", "") + "*))";
                SearchResult srfullName = dirSearcher.FindOne();
                string propName = "displayname";
                ResultPropertyValueCollection valColl = srfullName.Properties[propName];
                Session["fullname"] = Convert.ToString(valColl[0]);
            }
            fullname = Session["fullname"].ToString();
        }
    }
}