<%@ Page Title="Serial Checker" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SerialChecker._Default" EnableEventValidation="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function AdvanceSelect(id) {
            document.getElementById('MainContent_Wizard1_gvAdvanceCustomer_' + id).click();
        }
        function CRMSelect(id) {
            document.getElementById('MainContent_Wizard1_gvCRMCustomer_' + id).click();
        }
    </script>
    <div>
        <asp:Wizard ID="Wizard1" runat="server" DisplaySideBar="false" CssClass="col-sm-12" StepNextButtonStyle-CssClass="hidden" StepPreviousButtonStyle-CssClass="hidden"
            FinishPreviousButtonStyle-CssClass="hidden" StartNextButtonStyle-CssClass="hidden" FinishCompleteButtonStyle-CssClass="hidden" OnFinishButtonClick="Wizard1_FinishButtonClick">
            <HeaderTemplate>
                <div>
                    <ul class="steps">
                        <asp:Repeater ID="SideBarListSm" runat="server">
                            <ItemTemplate>
                                <li class="<%# GetClassForWizardStepSm(Container.DataItem) %>">
                                    <span style="cursor: default;" class="steps__link" title="<%# Eval("Name")%>"><%# Eval("Name")%></span>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </HeaderTemplate>
            <WizardSteps>
                <asp:WizardStep ID="WizardStep1" runat="server" Title="Enter Serial Number">
                    <div class="well well-lg" align="center">
                        <div class="form-inline">
                            <div class="form-group" id="divSearch" runat="server">
                                <label for="txtSerialNo" class="sr-only">Serial Number</label>
                                <asp:TextBox ID="txtSerialNo" runat="server" CssClass="form-control input-lg" placeholder="Serial Number"></asp:TextBox>
                            </div>
                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-lg btn-primary" OnClick="btnSearch_Click" />
                        </div>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Select Customer From Advance">
                    <asp:GridView ID="gvAdvanceCustomer" runat="server" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowHeader="false"
                        EmptyDataText="Customer not found." DataKeyNames="custNo, Name, BRN" OnRowCreated="gvAdvanceCustomer_RowCreated">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <div class="row">
                                        <asp:Button ID="btnSelect" runat="server" Text="Select" CssClass="btn btn-info btn-xs hidden" OnClick="btnSelect_Click" />
                                        <div class="col-sm-12">
                                            <div class="col-sm-12">
                                                <div class="col-sm-3">
                                                    <b>Company Name: </b>
                                                </div>
                                                <div class="col-sm-9">
                                                    <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-3">
                                                    <b>Contract: </b>
                                                </div>
                                                <div class="col-sm-9">
                                                    <asp:Label ID="lblContract" runat="server" Text='<%# Bind("contractNo") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-3">
                                                    <b>Model: </b>
                                                </div>
                                                <div class="col-sm-9">
                                                    <asp:Label ID="lblModel" runat="server" Text='<%# Bind("model") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-3">
                                                    <b>Item Number: </b>
                                                </div>
                                                <div class="col-sm-9">
                                                    <asp:Label ID="lblItemNo" runat="server" Text='<%# Bind("itemNo") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-3">
                                                    <b>BRN: </b>
                                                </div>
                                                <div class="col-sm-9">
                                                    <asp:Label ID="lblBRN" runat="server" Text='<%# Bind("BRN") %>'></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep3" runat="server" Title="Selet Customer From CRM">
                    <div>
                        <%--<asp:Button ID="btnNoMatch" runat="server" Text="None of above matches" CssClass="btn btn-primary btn-block" OnClick="btnNoMatch_Click" />--%>
                    </div>
                    <asp:GridView ID="gvCRMCustomer" runat="server" CssClass="table table-bordered table-hover" EmptyDataText="Customer not found."
                        AutoGenerateColumns="false" ShowHeader="false" OnRowCreated="gvCRMCustomer_RowCreated" DataKeyNames="name, brn, AdvanceCustNo, AdvanceCustName, AdvanceBRN">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <div>
                                        <asp:Button ID="btnSelectCRM" runat="server" Text="Select" CssClass="btn btn-info hidden" OnClick="btnSelectCRM_Click" />
                                        <div class="container-fluid">
                                            <div class="col-sm-12">
                                                <div class="col-sm-2">
                                                    <b>Company Name: </b>
                                                </div>
                                                <div class="col-sm-10">
                                                    <asp:Label ID="lblName" runat="server" Text='<%# Bind("name") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-2">
                                                    <b>BRN: </b>
                                                </div>
                                                <div class="col-sm-10">
                                                    <asp:Label ID="lblBRN" runat="server" Text='<%# Bind("brn") %>'></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep4" runat="server" Title="MIF Information">
                    <asp:Repeater runat="server" ID="gvMifInfo">
                        <ItemTemplate>
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h1><%# Eval("serialNo") %> <small runat="server"><%# Eval("name") %></small></h1>
                                </div>
                                <div class="panel-body">
                                    <dl class="dl-horizontal">
                                        <dt>Model</dt>
                                        <dd><span><%# Eval("model") %></span></dd>
                                        <dt>Installed Date</dt>
                                        <dd><span><%# Eval("installDate") %></span></dd>
                                        <dt>Contract</dt>
                                        <dd><span><%# Eval("contractNo") %></span></dd>
                                        <dd><span><%# Eval("contractType") %></span></dd>
                                        <dd><%# Eval("cntrctStart") %> - <%# Eval("cntrctEnd") %></dd>
                                        <dt>Contact</dt>
                                        <dd><span><%# Eval("contact") %></span></dd>
                                        <dt>Location</dt>
                                        <dd><span><%# Eval("location") %></span></dd>
                                    </dl>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="row">
                        <div class="col-md-4">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h4>Service History</h4>
                                </div>
                                <div class="panel-body">
                                    <ul class="list-group">
                                        <asp:Repeater runat="server" ID="gvServiceH">
                                            <ItemTemplate>
                                                <a class="list-group-item" href='<%# "#" + Eval("serviceNo") %>' data-toggle="collapse"
                                                    aria-expanded="false" aria-controls='<%# Eval("serviceNo") %>'>
                                                    <span><%# Eval("serviceNo") %></span><small class="pull-right text-muted"><%# Eval("entryDate") %></small>
                                                </a>
                                                <div class="collapse" id='<%# Eval("serviceNo") %>'>
                                                    <div class="well">
                                                        <dl class="dl">
                                                            <dt>Service Type</dt>
                                                            <dd><span><%# Eval("typeOfService") %></span></dd>
                                                            <dt>Technician</dt>
                                                            <dd><span><%# Eval("techCode") %> - <%# Eval("techName") %> - <%# Eval("team") %></span></dd>
                                                            <dt>Problem Description</dt>
                                                            <dd><span><%# Eval("problemDesc") %></span></dd>
                                                            <dt>Cause Description</dt>
                                                            <dd><span><%# Eval("causeDesc") %></span></dd>
                                                            <dt>Repair Description</dt>
                                                            <dd><span><%# Eval("repairDesc") %></span></dd>
                                                            <dt>Parts Used</dt>
                                                            <dd><span><%# Eval("parts") %></span></dd>
                                                            <dt>BW Reading</dt>
                                                            <dd><span><%# Eval("BWReading") %></span></dd>
                                                            <dt>COL Reading</dt>
                                                            <dd><span><%# Eval("COLReading") %></span></dd>
                                                            <dt>Entry Date/Time</dt>
                                                            <dd><span><%# Eval("entryDateTime") %></span></dd>
                                                            <dt>Complete Date/Time</dt>
                                                            <dd><span><%# Eval("completeDateTime") %></span></dd>
                                                        </dl>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h4>Toner Supply History</h4>
                                </div>
                                <div class="panel-body">
                                    <ul class="list-group">
                                        <asp:Repeater runat="server" ID="gvTonerH">
                                            <ItemTemplate>
                                                <a class="list-group-item" href='<%# "#" + Eval("serviceNo") %>' data-toggle="collapse"
                                                    aria-expanded="false" aria-controls='<%# Eval("serviceNo") %>'>
                                                    <span><%# Eval("serviceNo") %></span><small class="pull-right text-muted"><%# Eval("invoiceDate") %></small>
                                                </a>
                                                <div class="collapse" id='<%# Eval("serviceNo") %>'>
                                                    <div class="well">
                                                        <dl class="dl">
                                                            <dt>Toner Delivered</dt>
                                                            <dd><span><%# Eval("toners") %></span></dd>
                                                            <dt>Invoice Date</dt>
                                                            <dd><span><%# Eval("invoiceDate") %></span></dd>
                                                        </dl>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h4>Meter Reading History</h4>
                                </div>
                                <div class="panel-body">
                                    <ul class="list-group">
                                        <asp:Repeater runat="server" ID="gvMeterH">
                                            <ItemTemplate>
                                                <a class="list-group-item" href='<%# "#" + Eval("invoiceNo") %>' data-toggle="collapse"
                                                    aria-expanded="false" aria-controls='<%# Eval("invoiceNo") %>'>
                                                    <span><%# Eval("invoiceNo") %></span><small class="pull-right text-muted"><%# Eval("invoiceDate") %></small>
                                                </a>
                                                <div class="collapse" id='<%# Eval("invoiceNo") %>'>
                                                    <div class="well">
                                                        <dl class="dl">
                                                            <dt>BW Copies</dt>
                                                            <dd><span><%# Eval("bw") %></span></dd>
                                                            <dt>COL Copies</dt>
                                                            <dd><span><%# Eval("col") %></span></dd>
                                                        </dl>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:WizardStep>
            </WizardSteps>
            <StepNavigationTemplate>
                <asp:LinkButton ID="btnPrevious" runat="server" CssClass="btn btn-warning" CommandName="MovePrevious" CausesValidation="false">
                    <span class="icon icon-arrow-left"></span>
                    Previous
                </asp:LinkButton>
            </StepNavigationTemplate>
            <FinishNavigationTemplate>
                <asp:LinkButton ID="btnFinish" runat="server" CssClass="btn btn-success" CommandName="MoveComplete" CausesValidation="false">
                    <span class="icon icon-thumbs-up"></span>
                    Finish
                </asp:LinkButton>
            </FinishNavigationTemplate>
        </asp:Wizard>
    </div>
</asp:Content>
