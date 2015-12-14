using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SerialChecker
{
    public partial class _Default : Page
    {
        SqlConnection advanceConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["advance"].ConnectionString);
        public IList<Clients> clients = new List<Clients>();
        protected void Page_Load(object sender, EventArgs e)
        {
            Wizard1.PreRender += new EventHandler(Wizard1_PreRender);
        }
        protected void Wizard1_PreRender(object sender, EventArgs e)
        {
            Repeater SideBarListSm = Wizard1.FindControl("HeaderContainer").FindControl("SideBarListSm") as Repeater;
            SideBarListSm.DataSource = Wizard1.WizardSteps;
            SideBarListSm.DataBind();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSerialNo.Text.Length > 0)
            {
                string getSerialQuery = "SELECT cl.[contract-no] [contractNo] " +
                                         ", i2.[model] " +
                                         ", s.[item-no] [itemNo] " +
                                         ", s.[serial-no] [serialNo] " +
                                         ", c.[name] " +
                                         ", cs.[business-reg] AS [BRN] " +
                                         ", s.[cust-no] AS [custNo]" +
                                    "FROM serial AS s " +
                                    "INNER JOIN [contract-lin] AS cl ON cl.[serial-no] = s.[serial-no] " +
                                                                   "AND cl.[item-no] = s.[item-no] " +
                                                                   "AND cl.[cntrct-end] >= dbo.todate ( GETDATE())  " +
                                    "INNER JOIN customer AS c ON c.[cust-no] = s.[cust-no] " +
                                    "INNER JOIN [customer-supp] AS cs ON cs.[cust-no] = s.[cust-no] " +
                                    "INNER JOIN item2 AS i2 ON i2.[item-no] = s.[item-no] " +
                                    "LEFT JOIN [contract-ip] AS ci ON ci.[contract-no] = cl.[contract-no] " +
                                    "WHERE s.[serial-no] = '" + txtSerialNo.Text + "' " +
                                      "AND ci.[contract-no] IS NULL";
                gvAdvanceCustomer.DataSource = GetData(getSerialQuery);
                gvAdvanceCustomer.DataBind();

                divSearch.Attributes["class"] = "form-group";
                Wizard1.MoveTo(WizardStep2);
            }
            else
            {
                divSearch.Attributes["class"] = "form-group has-error has-feedback";
            }
        }
        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            GridViewRow row = btn.NamingContainer as GridViewRow;
            string custNo = gvAdvanceCustomer.DataKeys[row.RowIndex].Values[0].ToString();
            string custName = gvAdvanceCustomer.DataKeys[row.RowIndex].Values[1].ToString().Replace("&", "%26");
            string BRN = gvAdvanceCustomer.DataKeys[row.RowIndex].Values[2].ToString();

            string CRMCustomerInfo = GetCustomerInfo(custNo, custName, BRN);
            if (CRMCustomerInfo != "Customer not found.")
            {
                Customer customer = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Customer>(CRMCustomerInfo);
                for (int i = 0; i < customer.clients.Count; i++)
                {
                    string name = customer.clients[i].name;
                    string brn = customer.clients[i].brn;
                    clients.Add(new Clients
                    {
                        name = name,
                        brn = brn,
                        AdvanceCustNo = custNo,
                        AdvanceCustName = custName,
                        AdvanceBRN = BRN,
                    });
                }
            }
            else
            {
                clients = null;
            }

            gvCRMCustomer.DataSource = clients;
            gvCRMCustomer.DataBind();

            Wizard1.MoveTo(WizardStep3);
        }
        protected void btnSelectCRM_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            GridViewRow row = btn.NamingContainer as GridViewRow;

            string CRMCustName = gvCRMCustomer.DataKeys[row.RowIndex].Values[0].ToString();
            string CRMBRN = gvCRMCustomer.DataKeys[row.RowIndex].Values[1].ToString();
            string AdvanceCustNo = gvCRMCustomer.DataKeys[row.RowIndex].Values[2].ToString();
            string AdvanceCustName = gvCRMCustomer.DataKeys[row.RowIndex].Values[3].ToString();
            string AdvanceBRN = gvCRMCustomer.DataKeys[row.RowIndex].Values[4].ToString();

            GetMifInfo();

            Wizard1.MoveTo(WizardStep4);

            if (CRMBRN != AdvanceBRN)
            {
                Email(AdvanceCustNo, AdvanceCustName, CRMBRN);
            }
        }
        protected void Email(string custNo, string custName, string brn)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("zhaidy@ricoh.sg");
            mailMessage.To.Add("AdrianLim@ricoh.sg");
            mailMessage.From = new MailAddress("rspops@ricoh.sg", "Managed Application Services");
            mailMessage.Subject = "Customer BRN Update [RESTRICTED]";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "<span>Hi All,</span></br></br>" +
                               "<span>Please update the BRN number accordingly.</span></br></br>" +
                               "<table>" +
                                  "<tr>" +
                                    "<td><span>Customer Number : </span></td>" +
                                    "<td><b>" + custNo + "</b></td>" +
                                    "<td>" +
                                  "</tr>" +
                                  "<tr>" +
                                    "<td><span>Customer Name : </span></td>" +
                                    "<td><b>" + custName + "</b></td>" +
                                    "<td>" +
                                  "</tr>" +
                                  "<tr>" +
                                    "<td><span>BRN : </span></td>" +
                                    "<td><b>" + brn + "</b></td>" +
                                    "<td>" +
                                  "</tr>" +
                               "</table></br></br>" +
                               "<span>Regards, </span></br>" +
                               "<span>Managed Application Services";
            SmtpClient smtpClient = new SmtpClient("172.19.107.45");
            smtpClient.Send(mailMessage);
        }
        protected void GetMifInfo()
        {
            string getMifInfoCmd = "SELECT mt.[serial-no] AS   [serialNo] " +
                                        ", mt.[contract-no] AS [contractNo] " +
                                        ", c.name " +
                                        ", s.[location] " +
                                        ", ct.[contact] " +
                                        ", CONVERT( VARCHAR(10), ct.[cntrct-end], 103) AS [cntrctEnd] " +
                                        ", i2.[model] " +
                                   "FROM datacenter.dbo.mif_tdv AS mt " +
                                   "INNER JOIN customer AS c ON c.[cust-no] = mt.[cust-no] " +
                                   "INNER JOIN serial AS s ON s.[serial-no] = mt.[serial-no] AND s.[item-no] = mt.[item-no]" +
                                   "INNER JOIN contract AS ct ON ct.[contract-no] = mt.[contract-no] " +
                                   "INNER JOIN item2 AS i2 ON i2.[item-no] = mt.[item-no] " +
                                   "WHERE mt.[trans-date] = datacenter.dbo.firstdayofpreviousmonth(getdate()) " +
                                     "AND mt.[serial-no] = '" + txtSerialNo.Text + "'";
            gvMifInfo.DataSource = GetData(getMifInfoCmd);
            gvMifInfo.DataBind();
        }
        protected string GetCustomerInfo(string custNo, string custName, string BRN)
        {
            string url = "https://spangular.ricohmds.sg/api/crm/clients?auth_token=A12345678&code=" + custNo + "&name=" + custName + "&brn=" + BRN;
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Console.WriteLine(response.StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
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
        protected string GetClassForWizardStepSm(object wizardStep)
        {
            WizardStep step = wizardStep as WizardStep;

            if (step == null)
            {
                return "";
            }
            int stepIndex = Wizard1.WizardSteps.IndexOf(step);
            string stepString = "";

            if (stepIndex == 0)
            {
                stepString = " steps__item--first";
            }

            if (stepIndex == 3)
            {
                stepString = " steps__item--last";
            }

            if (stepIndex < Wizard1.ActiveStepIndex)
            {
                //return "prevStep";
                return "steps__item steps__item--done" + stepString;
            }
            else if (stepIndex > Wizard1.ActiveStepIndex)
            {
                //return "nextStep";
                return "steps__item" + stepString;
            }
            else
            {
                //return "currentStep";
                return "steps__item steps__item--active" + stepString;
            }
        }
        protected string GetClassForWizardStepLg(object wizardStep)
        {
            WizardStep step = wizardStep as WizardStep;

            if (step == null)
            {
                return "";
            }
            int stepIndex = Wizard1.WizardSteps.IndexOf(step);

            if (stepIndex < Wizard1.ActiveStepIndex)
            {
                return "prevStep";
                //return "";
            }
            else if (stepIndex > Wizard1.ActiveStepIndex)
            {
                return "nextStep";
                //return "";
            }
            else
            {
                return "currentStep";
                //return "active";
            }
        }
        protected void gvAdvanceCustomer_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnSelect = e.Row.FindControl("btnSelect") as Button;
                e.Row.Attributes["onclick"] = "AdvanceSelect('" + btnSelect.ClientID + "')";
                e.Row.Attributes["style"] = "cursor:pointer";
            }
        }

        protected void gvCRMCustomer_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnSelectCRM = e.Row.FindControl("btnSelectCRM") as Button;
                e.Row.Attributes["onclick"] = "CRMSelect('" + btnSelectCRM.ClientID + "')";
                e.Row.Attributes["style"] = "cursor:pointer";
            }
        }

        protected void Wizard1_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void btnNoMatch_Click(object sender, EventArgs e)
        {
            GetMifInfo();
            Wizard1.MoveTo(WizardStep4);
        }
    }
    
    public class Customer
    {
        public List<Clients> clients { get; set; }
    }
    public class Clients
    {
        public string AdvanceCustNo { get; set; }
        public string AdvanceCustName { get; set; }
        public string name { get; set; }
        public string brn { get; set; }
        public string AdvanceBRN { get; set; }
    }
}