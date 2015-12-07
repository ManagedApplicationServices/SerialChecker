using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SerialChecker
{
    public partial class _Default : Page
    {
        SqlConnection advanceConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["advance"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string getSerialQuery = "SELECT cl.[contract-no] [contractNo] " +
                                         ", i2.[model] " +
                                         ", cl.[item-no] [itemNo] " +
                                         ", s.[serial-no] [serialNo] " +
                                         ", c.[name] " +
                                         ", cs.[business-reg] AS [BRN] " +
                                    "FROM serial AS s " +
                                    "INNER JOIN [contract-lin] AS cl ON cl.[serial-no] = s.[serial-no] " +
                                                                   "AND cl.[cntrct-end] >= dbo.todate ( GETDATE())  " +
                                    "INNER JOIN customer AS c ON c.[cust-no] = s.[cust-no] " +
                                    "INNER JOIN [customer-supp] AS cs ON cs.[cust-no] = s.[cust-no] " +
                                    "INNER JOIN item2 AS i2 ON i2.[item-no] = cl.[item-no] " +
                                    "LEFT JOIN [contract-ip] AS ci ON ci.[contract-no] = cl.[contract-no] " +
                                    "WHERE s.[serial-no] = '" + txtSerialNo.Text + "' " +
                                      "AND ci.[contract-no] IS NULL";
            gvSearchResult.DataSource = GetData(getSerialQuery);
            gvSearchResult.DataBind();
        }

        private static DataTable GetData(string query)
        {
            string strConnString = ConfigurationManager.ConnectionStrings["advance"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = query;
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }
    }
}