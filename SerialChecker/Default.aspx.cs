using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SerialChecker
{
    public partial class _Default : Page
    {
        public string custName = "";
        public string brn = "";
        public string custNo = "";
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

                if (GetData(getSerialQuery).Rows.Count == 0)
                {
                    divAdvanceMsg.Attributes.Add("class", "show alert alert-warning");
                }
                else
                {
                    divAdvanceMsg.Attributes.Add("class", "hidden");
                }
            }
            else
            {
                divSearch.Attributes["class"] = "form-group has-error has-feedback";
            }
        }
        protected void lbSelectAdvance_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            HiddenField hfCustNo = btn.FindControl("hfCustNo") as HiddenField;
            HiddenField hfName = btn.FindControl("hfName") as HiddenField;
            HiddenField hfBRN = btn.FindControl("hfBRN") as HiddenField;

            string custNo = hfCustNo.Value;
            string custName = hfName.Value;
            string BRN = hfBRN.Value;
            string adName = Page.User.Identity.Name.Replace(@"RSP\", "");

            string CRMCustomerInfo = GetCustomerInfo(custNo, custName, BRN, adName);

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
                divCRMCustomer.Attributes.Add("class", "hidden");
            }
            else
            {
                clients = null;
                divCRMCustomer.Attributes.Add("class", "show alert alert-warning");
            }

            gvCRMCustomer.DataSource = clients;
            gvCRMCustomer.DataBind();

            Wizard1.MoveTo(WizardStep3);
        }
        protected void lbSelectCRM_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            HiddenField hfName = btn.FindControl("hfName") as HiddenField;
            HiddenField hfBrn = btn.FindControl("hfBrn") as HiddenField;
            HiddenField hfAdvanceCustNo = btn.FindControl("hfAdvanceCustNo") as HiddenField;
            HiddenField hfAdvanceCustName = btn.FindControl("hfAdvanceCustName") as HiddenField;
            HiddenField hfAdvanceBRN = btn.FindControl("hfAdvanceBRN") as HiddenField;

            string CRMCustName = hfName.Value;
            string CRMBRN = hfBrn.Value;
            string AdvanceCustNo = hfAdvanceCustNo.Value;
            string AdvanceCustName = hfAdvanceCustName.Value;
            string AdvanceBRN = hfAdvanceBRN.Value;

            GetMifInfo();

            hfCustName.Value = AdvanceCustName;
            hfCustNo.Value = AdvanceCustNo;
            hfBRN.Value = AdvanceBRN;
            brn = AdvanceBRN;
            
            mvResult.ActiveViewIndex = 0;
            Wizard1.MoveTo(WizardStep4);

            if (CRMBRN != AdvanceBRN)
            {
                Email(AdvanceCustNo, AdvanceCustName, CRMBRN);
            }
        }
        protected void btnShowCust_Click(object sender, EventArgs e)
        {
            brn = hfBRN.Value;
            if (brn.Length > 0)
            {
                string sql = "SELECT DISTINCT\n"
                            + "       mt.[serial-no] AS [serialNo]\n"
                            + "FROM   [datacenter].[dbo].[MIF_TDV] AS [mt]\n"
                            + "INNER JOIN [contract-lin] AS [cl] ON [cl].[serial-no] = [mt].[serial-no] \n"
                            + "                                     AND [cl].[item-no] = [mt].[item-no] \n"
                            + "                                     AND [cl].[cntrct-end] >= [dbo].[todate](GETDATE())\n"
                            + "WHERE  [mt].[cust-no] = (SELECT TOP 1 [cs].[Cust-no]\n"
                            + "                         FROM         [Customer-Supp] AS [cs]\n"
                            + "                         WHERE        [cs].[Business-reg] = '" + brn + "')\n"
                            + "       AND [mt].[trans-date] = [datacenter].[dbo].[firstdayofpreviousmonth](GETDATE());";
                gvSerialList.DataSource = GetData(sql);
                gvSerialList.DataBind();
            }
            else
            {
                brn = "No brn found, please contact AR team to update";
            }
            mvResult.ActiveViewIndex = 1;
        }
        protected void lbSerial_Click(object sender, EventArgs e)
        {
            LinkButton lb = sender as LinkButton;
            string serialNo = lb.Text;
            txtSerialNo.Text = serialNo;
            GetMifInfo();
            mvResult.ActiveViewIndex = 0;
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            mvResult.ActiveViewIndex = 0;
        }
        protected void Email(string custNo, string custName, string brn)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("zhaidy@ricoh.sg");
            //mailMessage.To.Add("ar@ricoh.sg");
            //mailMessage.CC.Add("zhaidy@ricoh.sg");
            //mailMessage.To.Add("AdrianLim@ricoh.sg");
            mailMessage.From = new MailAddress("rspops@ricoh.sg", "Managed Application Services");
            mailMessage.Subject = "Customer BRN Update [RESTRICTED]";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "<span>Hi All,</span></br></br>" +
                               "<span>Please update the BRN number accordingly in Advance.</span></br></br>" +
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
                               "<span><a href='https://mobileservice.ricoh.sg/SerialChecker/WrongBRN?name=" + custName + "&custNo=" + custNo + "&brn=" + brn + "'>Click me if the BRN is not correct</a></span></br></br>" +
                               "<span>Regards, </span></br>" +
                               "<span>Managed Application Services";
            SmtpClient smtpClient = new SmtpClient("172.19.107.45");
            smtpClient.Send(mailMessage);
        }
        protected void GetMifInfo()
        {
            //string getBrnSerialCount = "SELECT COUNT([mt].[serial-no]) AS [serialNo]\n"
            //           + "FROM   [datacenter].[dbo].[MIF_TDV] AS [mt]\n"
            //           + "INNER JOIN [contract-lin] AS [cl] ON [cl].[serial-no] = [mt].[serial-no]\n"
            //           + "                                     AND [cl].[item-no] = [mt].[item-no]\n"
            //           + "                                     AND [cl].[cntrct-end] >= [dbo].[todate](GETDATE())\n"
            //           + "WHERE  [mt].[cust-no] = (SELECT TOP 1 [cs].[Cust-no]\n"
            //           + "                         FROM         [Customer-Supp] AS [cs]\n"
            //           + "                         WHERE        [cs].[Business-reg] = 'T15FC0083A')\n"
            //           + "       AND [mt].[trans-date] = [datacenter].[dbo].[firstdayofpreviousmonth](GETDATE())";
            //SqlCommand getBrnSerialCountCmd = new SqlCommand(getBrnSerialCount, advanceConn);
            //advanceConn.Open();
            //int count = Convert.ToInt32(getBrnSerialCountCmd.ExecuteScalar());
            //advanceConn.Close();

            string getMifInfoCmd = "SELECT mt.[serial-no] AS                                [serialNo] " +
                                        ", mt.[contract-no] AS                              [contractNo] " +
                                        ", ct.[contract-typ] AS						        [contractType] " +
                                        ", c.name AS                                        [name] " +
                                        ", s.[location] AS                                  [location] " +
                                        ", CONVERT( VARCHAR, s.[install-date], 103) AS      [installDate] " +
                                        ", ct.[contact] AS                                  [contact] " +
                                        ", CONVERT( VARCHAR(10), cl.[cntrct-start], 103) AS [cntrctStart] " +
                                        ", CONVERT( VARCHAR(10), cl.[cntrct-end], 103) AS   [cntrctEnd] " +
                                        ", i2.[model] AS                                    [model] " +
                                   "FROM datacenter.dbo.mif_tdv AS mt " +
                                   "INNER JOIN customer AS c ON c.[cust-no] = mt.[cust-no] " +
                                   "INNER JOIN serial AS s ON s.[serial-no] = mt.[serial-no] AND s.[item-no] = mt.[item-no] " +
                                   "INNER JOIN contract AS ct ON ct.[contract-no] = mt.[contract-no] " +
                                   "INNER JOIN [contract-lin] AS [cl] ON [cl].[serial-no] = [s].[serial-no] " +
                                     "AND [cl].[item-no] = [s].[item-no] " +
                                     "AND [cl].[cntrct-end] >= [dbo].[todate](GETDATE()) " +
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
                                         ", isnull((select top 1 smh.[meter_reading] from smeter_history smh " +
                                             "where smh.[service_no] = s.[service-no] and " + 
                                                   "smh.[meter_type] like 'B%'),0) AS                                                                                                            [BWReading] " +
                                         ", isnull((select top 1 smh.[meter_reading] from smeter_history smh " +
                                             "where smh.[service_no] = s.[service-no] and smh.[meter_type] like 'C%'),0) AS                                                                      [COLReading] " +
                                         ", CONVERT( VARCHAR, s.[entry-date], 103) AS                                                                                                         [entryDate] " +
                                         ", CONVERT( VARCHAR, s.[entry-date], 103) + ' ' + LEFT(CONVERT(VARCHAR, s.[entry-time]), 2) + ':' + RIGHT(CONVERT(VARCHAR, s.[entry-time]), 2) AS    [entryDateTime] " +
                                         ", CONVERT( VARCHAR, s.[complete-dt], 103) + ' ' + LEFT(CONVERT(VARCHAR, s.[complete-tm]), 2) + ':' + RIGHT(CONVERT(VARCHAR, s.[complete-tm]), 2) AS [completeDateTime] " +
                                         ", CASE WHEN dbo.SL_Parts_List2(s.[service-no]) = '' then 'No parts used' ELSE dbo.SL_Parts_List2(s.[service-no]) END AS                                                                                                             [parts] " +
                                    "FROM service AS s " +
                                    "INNER JOIN item AS i ON i.[item-no] = s.[item-no] " +
                                    "INNER JOIN item2 AS i2 ON i2.[item-no] = s.[item-no] " +
                                    "WHERE s.[serial-no] = '" + txtSerialNo.Text + "' " +
                                      "AND s.[so-status] LIKE 'F90' " +
                                      "AND s.[entry-date] >= DATEADD(m, -6, GETDATE()) " +
                                    "ORDER BY [entry-date] DESC";
            gvServiceH.DataSource = GetData(getServiceHCmd);
            gvServiceH.DataBind();

            string getTonerHCmd = "SELECT s.[service-no] AS                             [serviceNo] " +
                                       ", replace(dbo.SL_Parts_List2(s.[service-no]),',','<br>') AS      [toners] " +
                                       ", CONVERT( VARCHAR, s.[invoice-date], 103) AS   [invoiceDate] " +
                                    "FROM service AS s " +
                                    "WHERE s.[serial-no] = '" + txtSerialNo.Text + "' " +
                                      "AND s.[so-status] LIKE 'P90' " +
                                      "AND s.[invoice-date] >= DATEADD(m, -6, GETDATE())  " +
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

            BindServiceHChart();
            BindTonerHChart();
            BindMeterHChart();
        }
        protected string GetCustomerInfo(string custNo, string custName, string BRN, string adName)
        {
            string url = "https://spanif.ricohmds.sg/api/crm/clients?auth_token=10bf3c81354c592b95ccffdefd644887&code=" + custNo + "&name=" + custName + "&brn=" + BRN + "&ad_name=" + adName;
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
        protected void Wizard1_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }
        protected void btnNoMatch_Click(object sender, EventArgs e)
        {
            Wizard1.MoveTo(WizardStep4);
        }
        protected void BindServiceHChart()
        {
            DataTable dsChartData = new DataTable();
            StringBuilder strScript = new StringBuilder();
            string query = "SELECT ( SELECT COUNT(s1.[service-no]) " +
                                     "FROM service AS s1 " +
                                     "WHERE s1.[serial-no] = '" + txtSerialNo.Text + "' " +
                                       "AND s1.[so-status] LIKE 'F90' " +
                                       "AND s1.[entry-date] BETWEEN DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[entry-date]), 0) AND datacenter.dbo.lastdayofmonth ( DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[entry-date]), 0)) " +
                                       "AND s1.[service-type] IN ( 'APFC', 'APFW', 'APRT-EXX', 'APRT-OTH', 'FC', 'FW', 'RT-EXXON', 'RT-O', 'RT-OTHER', 'PPP', 'APPPP' )) AS                                                                            [EM] " +
                                 ", ( SELECT COUNT(s1.[service-no])  " +
                                     "FROM service AS s1 " +
                                     "WHERE s1.[serial-no] = '" + txtSerialNo.Text + "' " +
                                       "AND s1.[so-status] LIKE 'F90' " +
                                       "AND s1.[entry-date] BETWEEN DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[entry-date]), 0) AND datacenter.dbo.lastdayofmonth ( DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[entry-date]), 0)) " +
                                       "AND s1.[service-type] IN ( 'PM', 'RCFC', 'RCFW', 'RCRT-EXX', 'RCRT-OTH', 'SCFC', 'SCFW', 'RCPPP' )) AS                                                                                                         [PM] " +
                                 ", ( SELECT COUNT(s1.[service-no])  " +
                                     "FROM service AS s1 " +
                                     "WHERE s1.[serial-no] = '" + txtSerialNo.Text + "' " +
                                       "AND s1.[so-status] LIKE 'F90' " +
                                       "AND s1.[entry-date] BETWEEN DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[entry-date]), 0) AND datacenter.dbo.lastdayofmonth ( DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[entry-date]), 0))  " +
                                       "AND s1.[service-type] NOT IN ( 'APFC', 'APFW', 'APRT-EXX', 'APRT-OTH', 'FC', 'FW', 'RT-EXXON', 'RT-O', 'RT-OTHER', 'PPP', 'APPPP', 'PM', 'RCFC', 'RCFW', 'RCRT-EXX', 'RCRT-OTH', 'SCFC', 'SCFW', 'RCPPP' )) AS [OTHERS] " +
                                 ", CONVERT( CHAR(3), s.[entry-date], 0) + ' ' + CONVERT(CHAR(4), s.[entry-date], 120) AS                                                                                                                              [Month] " +
                                 ", count(s.[service-no]) [Total] " +
                            "FROM service AS s " +
                            "WHERE s.[serial-no] = '" + txtSerialNo.Text + "' " +
                              "AND s.[so-status] LIKE 'F90' " +
                              "AND s.[entry-date] >= DATEADD(m, -6, GETDATE()) " +
                            "GROUP BY DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[entry-date]), 0) " +
                            ", CONVERT( CHAR(3), s.[entry-date], 0) + ' ' + CONVERT(CHAR(4), s.[entry-date], 120) " +
                            "order by DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[entry-date]), 0)";
            try
            {
                dsChartData = GetData(query);
                if (dsChartData.Rows.Count > 0)
                {
                    strScript.Append(@"<script type='text/javascript'>  
                    google.load('visualization', '1', {packages: ['corechart']});</script>  
                    <script type='text/javascript'>  
                    function drawVisualization() {         
                    var data = google.visualization.arrayToDataTable([  
                    ['Month', 'PM', 'EM', 'OTHERS', 'Total'],");
                    foreach (DataRow row in dsChartData.Rows)
                    {
                        strScript.Append("['" + row["Month"] + "'," + row["PM"] + "," +
                            row["EM"] + "," + row["OTHERS"] + "," + row["Total"] + "],");
                    }
                    strScript.Remove(strScript.Length - 1, 1);
                    strScript.Append("]);");
                    strScript.Append("var options = { title : 'Service History - Last 6 Months', vAxis: {title: 'Service Count'},  hAxis: {title: 'Month'}, seriesType: 'bars', series: {3: {type: 'line'}}, legend: { position: 'top' } };");
                    strScript.Append(" var chart = new google.visualization.ComboChart(document.getElementById('serviceHChartDiv'));  chart.draw(data, options); } google.setOnLoadCallback(drawVisualization);");
                    strScript.Append(" </script>");
                    ltServiceHScript.Text = strScript.ToString();
                }
                else
                {

                }
            }
            catch
            {
            }
            finally
            {
                dsChartData.Dispose();
                strScript.Clear();
            }
        }
        protected void BindTonerHChart()
        {
            DataTable dsChartData = new DataTable();
            StringBuilder strScript = new StringBuilder();
            string query = "SELECT SUM(sl.[qty-shipped]) AS                                                                   [Toner]" +
                                 ", CONVERT( CHAR(3), s.[invoice-date], 0) + ' ' + CONVERT(CHAR(4), s.[invoice-date], 120) AS [Month]" +
                            "FROM service AS s " +
                            "INNER JOIN [service-line] AS sl ON sl.[service-no] = s.[service-no] " +
                            "WHERE s.[serial-no] = '" + txtSerialNo.Text + "' " +
                              "AND s.[so-status] LIKE 'P90' " +
                              "AND s.[invoice-date] >= DATEADD(m, -6, GETDATE()) " +
                            "GROUP BY DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[invoice-date]), 0) " +
                                   ", CONVERT( CHAR(3), s.[invoice-date], 0) + ' ' + CONVERT(CHAR(4), s.[invoice-date], 120) " +
                            "order by DATEADD(MONTH, DATEDIFF(MONTH, 0, s.[invoice-date]), 0)";
            try
            {
                dsChartData = GetData(query);
                if (dsChartData.Rows.Count > 0)
                {
                    strScript.Append(@"<script type='text/javascript'>  
                    google.load('visualization', '1', {packages: ['corechart']});</script>  
                    <script type='text/javascript'>  
                    function drawVisualization() {         
                    var data = google.visualization.arrayToDataTable([  
                    ['Month', 'Toner Supplied'],");
                    foreach (DataRow row in dsChartData.Rows)
                    {
                        strScript.Append("['" + row["Month"] + "'," + row["Toner"] + "],");
                    }
                    strScript.Remove(strScript.Length - 1, 1);
                    strScript.Append("]);");
                    strScript.Append("var options = { title : 'Toner Supply History - Last 6 Months', vAxis: {title: 'Toner Supplied'},  hAxis: {title: 'Month'}, seriesType: 'bars', legend: { position: 'top' } };");
                    strScript.Append(" var chart = new google.visualization.ComboChart(document.getElementById('tonerHChartDiv'));  chart.draw(data, options); } google.setOnLoadCallback(drawVisualization);");
                    strScript.Append(" </script>");
                    ltTonerHScript.Text = strScript.ToString();
                }
                else
                {

                }
            }
            catch
            {
            }
            finally
            {
                dsChartData.Dispose();
                strScript.Clear();
            }
        }
        protected void BindMeterHChart()
        {
            DataTable dsChartData = new DataTable();
            StringBuilder strScript = new StringBuilder();
            string query = "SELECT SUM(CASE " +
                                           "WHEN meter_type LIKE 'B%' " +
                                           "THEN [copies] " +
                                           "ELSE 0 " +
                                       "END) AS [BW] " +
                                 ", SUM(CASE " +
                                           "WHEN meter_type LIKE 'C%' " +
                                           "THEN [copies] " +
                                           "ELSE 0 " +
                                       "END) AS [COL] " +
                                 ", CONVERT( CHAR(3), [invoice-date], 0) + ' ' + CONVERT(CHAR(4), [invoice-date], 120) [Month]" +
                            "FROM [meter-card] " +
                            "WHERE [serial-no] = '" + txtSerialNo.Text + "' " +
                              "AND [invoice-date] >= DATEADD(m, -6, GETDATE())  " +
                            "GROUP BY DATEADD(MONTH, DATEDIFF(MONTH, 0, [invoice-date]), 0)  " +
                                   ", CONVERT( CHAR(3), [invoice-date], 0) + ' ' + CONVERT(CHAR(4), [invoice-date], 120)  " +
                            "ORDER BY DATEADD(MONTH, DATEDIFF(MONTH, 0, [invoice-date]), 0)";
            try
            {
                dsChartData = GetData(query);
                if (dsChartData.Rows.Count > 0)
                {
                    strScript.Append(@"<script type='text/javascript'>  
                    google.load('visualization', '1', {packages: ['corechart']});</script>  
                    <script type='text/javascript'>  
                    function drawVisualization() {         
                    var data = google.visualization.arrayToDataTable([  
                    ['Month', 'BW Copies', 'Color Copies'],");
                    foreach (DataRow row in dsChartData.Rows)
                    {
                        strScript.Append("['" + row["Month"] + "'," + row["BW"] + "," + row["COL"] + "],");
                    }
                    strScript.Remove(strScript.Length - 1, 1);
                    strScript.Append("]);");
                    strScript.Append("var options = { title : 'Meter Reading - Last 6 Months', vAxis: {title: 'Copies'},  hAxis: {title: 'Month'}, legend: { position: 'top' } };");
                    strScript.Append(" var chart = new google.visualization.LineChart(document.getElementById('meterHChartDiv'));  chart.draw(data, options); } google.setOnLoadCallback(drawVisualization);");
                    strScript.Append(" </script>");
                    ltMeterHScript.Text = strScript.ToString();
                }
                else
                {

                }
            }
            catch
            {
            }
            finally
            {
                dsChartData.Dispose();
                strScript.Clear();
            }
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