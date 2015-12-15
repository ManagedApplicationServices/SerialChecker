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

            GetMifInfo(AdvanceCustNo);

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
            //mailMessage.To.Add("AdrianLim@ricoh.sg");
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
                               "</table></br>" +
                               "<span><a href='http://172.19.107.84/SerialChecker/WrongBRN?name=" + custName + "'>Click me if the BRN is not correct</a></span></br></br>" +
                               "<span>Regards, </span></br>" +
                               "<span>Managed Application Services";
            SmtpClient smtpClient = new SmtpClient("172.19.107.45");
            smtpClient.Send(mailMessage);
        }
        protected void GetMifInfo(string custNo)
        {
            string getMifInfoCmd = "SELECT mt.[serial-no] AS                                [serialNo] " +
                                        ", mt.[contract-no] AS                              [contractNo] " +
                                        ", ct.[contract-typ] AS						        [contractType] " +
                                        ", c.name AS                                        [name] " +
                                        ", s.[location] AS                                  [location] " +
                                        ", CONVERT( VARCHAR, s.[install-date], 103) AS      [installDate] " +
                                        ", ct.[contact] AS                                  [contact] " +
                                        ", CONVERT( VARCHAR(10), ct.[cntrct-start], 103) AS [cntrctStart] " +
                                        ", CONVERT( VARCHAR(10), ct.[cntrct-end], 103) AS   [cntrctEnd] " +
                                        ", i2.[model] AS                                    [model] " +
                                   "FROM datacenter.dbo.mif_tdv AS mt " +
                                   "INNER JOIN customer AS c ON c.[cust-no] = mt.[cust-no] " +
                                   "INNER JOIN serial AS s ON s.[serial-no] = mt.[serial-no] AND s.[item-no] = mt.[item-no] " +
                                   "INNER JOIN contract AS ct ON ct.[contract-no] = mt.[contract-no] " +
                                   "INNER JOIN item2 AS i2 ON i2.[item-no] = mt.[item-no] " +
                                   "WHERE mt.[trans-date] = datacenter.dbo.firstdayofpreviousmonth(getdate()) " +
                                     "AND mt.[serial-no] = '" + txtSerialNo.Text + "'";
            gvMifInfo.DataSource = GetData(getMifInfoCmd);
            gvMifInfo.DataBind();

            string getServiceHCmd = "SELECT s.[service-no] AS                                                                                                                                 [serviceNo] " +
                                         ", CASE " +
                                               "WHEN s.[problem-code] IN ( 'P02', 'F100', 'F101', 'D11', 'E09', 'E10' )  " +
                                               "THEN 'INS' " +
                                               "WHEN s.[service-type] IN ( 'APFC', 'APFW', 'APRT-EXX', 'APRT-OTH', 'FC', 'FW', 'RT-EXXON', 'RT-O', 'RT-OTHER', 'PPP', 'APPPP' )  " +
                                               "THEN 'EM' " +
                                               "WHEN s.[service-type] IN ( 'PM', 'RCFC', 'RCFW', 'RCRT-EXX', 'RCRT-OTH', 'SCFC', 'SCFW', 'RCPPP' )  " +
                                               "THEN 'PM' " +
                                               "ELSE ( 'OTHERS' )  " +
                                           "END AS                                                                                                                                            [typeOfService] " +
                                         ", ( SELECT TOP 1 UPPER(tt.team)  " +
                                             "FROM datacenter.dbo.[tech_team] AS tt " +
                                             "WHERE tt.[tech-code] = s.[tech-code] ) AS                                                                                                       [team] " +
                                         ", UPPER(s.[tech-code]) AS                                                                                                                           [techCode] " +
                                         ", ( SELECT TOP 1 UPPER(tt.[tech-name])  " +
                                             "FROM datacenter.dbo.[tech_team] AS tt " +
                                             "WHERE tt.[tech-code] = s.[tech-code] ) AS                                                                                                       [techName] " +
                                         ", (select top 1 c.[column 2] from codes c " +
                                             "where c.[column 0] = 'SOPT' and " +
                                             "c.[column 1] = s.[problem-code]) AS                                                                                                             [problemDesc] " +
                                         ", (select top 1 c.[column 2] from codes c " +
                                             "where c.[column 0] = 'SOPC' and " +
                                                   "c.[column 1] = s.[cause-code]) AS                                                                                                         [causeDesc] " +
                                         ", (select top 1 c.[column 2] from codes c " +
                                             "where c.[column 0] = 'SOPR' and " +
                                                   "c.[column 1] = s.[repair-code]) AS                                                                                                        [repairDesc] " +
                                         ", (select top 1 smh.[meter_reading] from smeter_history smh " +
                                             "where smh.[service_no] = s.[service-no] and " + 
                                                   "smh.[meter_type] like 'B%') AS                                                                                                            [BWReading] " +
                                         ", (select top 1 smh.[meter_reading] from smeter_history smh " +
                                             "where smh.[service_no] = s.[service-no] and smh.[meter_type] like 'C%') AS                                                                      [COLReading] " +
                                         ", CONVERT( VARCHAR, s.[entry-date], 103) AS                                                                                                         [entryDate] " +
                                         ", CONVERT( VARCHAR, s.[entry-date], 103) + ' ' + LEFT(CONVERT(VARCHAR, s.[entry-time]), 2) + ':' + RIGHT(CONVERT(VARCHAR, s.[entry-time]), 2) AS    [entryDateTime] " +
                                         ", CONVERT( VARCHAR, s.[complete-dt], 103) + ' ' + LEFT(CONVERT(VARCHAR, s.[complete-tm]), 2) + ':' + RIGHT(CONVERT(VARCHAR, s.[complete-tm]), 2) AS [completeDateTime] " +
                                         ", CASE WHEN dbo.SL_Parts_List2(s.[service-no]) = '' then 'No parts used' ELSE dbo.SL_Parts_List2(s.[service-no]) END AS                                                                                                             [parts] " +
                                    "FROM service AS s " +
                                    "INNER JOIN item AS i ON i.[item-no] = s.[item-no] " +
                                    "INNER JOIN item2 AS i2 ON i2.[item-no] = s.[item-no] " +
                                    "WHERE s.[serial-no] = '" + txtSerialNo.Text + "' " +
                                      "AND s.[cust-no] = '" + custNo + "' " +
                                      "AND s.[so-status] LIKE 'F90' " +
                                      "AND s.[entry-date] >= DATEADD(m, -6, GETDATE()) " +
                                    "ORDER BY [entry-date] DESC";
            gvServiceH.DataSource = GetData(getServiceHCmd);
            gvServiceH.DataBind();

            string getTonerHCmd = "SELECT s.[service-no] AS                             [serviceNo] " +
                                       ", dbo.SL_Parts_List2(s.[service-no]) AS      [toners] " +
                                       ", CONVERT( VARCHAR, s.[invoice-date], 103) AS   [invoiceDate] " +
                                    "FROM service AS s " +
                                    "WHERE s.[serial-no] = 'M6201800084' " +
                                      "AND s.[cust-no] = 'KE074700' " +
                                      "AND s.[so-status] LIKE 'P90' " +
                                      "AND s.[entry-date] >= DATEADD(m, -6, GETDATE())  " +
                                    "ORDER BY s.[invoice-date] DESC ";
            gvTonerH.DataSource = GetData(getTonerHCmd);
            gvTonerH.DataBind();

            string getMeterHCmd = "SELECT [invoice-no] AS                                               [invoiceNo] " +
                                        ", MAX(case when meter_type like 'B%' then [copies] else 0 end) [bw] " +
                                        ", MAX(case when meter_type like 'C%' then [copies] else 0 end) [col] " +
                                        ", CONVERT( VARCHAR, [invoice-date], 103)                       [invoiceDate] " +
                                  "FROM [meter-card] " +
                                  "WHERE [serial-no] = '" + txtSerialNo.Text + "' " +
                                    "AND [invoice-date] >= DATEADD(m, -6, GETDATE())  " +
                                  "GROUP BY [invoice-no] " +
                                         ", [invoice-date] " +
                                  "ORDER BY [invoice-date] DESC";
            gvMeterH.DataSource = GetData(getMeterHCmd);
            gvMeterH.DataBind();
        }
        protected string GetCustomerInfo(string custNo, string custName, string BRN)
        {
            string url = "https://spangular.ricohmds.sg/api/crm/clients?auth_token=A12345678&code=" + custNo + "&name=" + custName + "&brn=" + BRN;
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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