﻿<%@ Page Title="Serial Checker" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="SerialChecker._Default" EnableEventValidation="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Wizard ID="Wizard1" runat="server" DisplaySideBar="false" CssClass="col-sm-12"
            StepNextButtonStyle-CssClass="hidden" StepPreviousButtonStyle-CssClass="hidden"
            FinishPreviousButtonStyle-CssClass="hidden" StartNextButtonStyle-CssClass="hidden"
            FinishCompleteButtonStyle-CssClass="hidden" OnFinishButtonClick="Wizard1_FinishButtonClick">
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
                    <asp:Panel runat="server" DefaultButton="btnSearch">
                        <div class="panel panel-default">
                            <div class="panel-body text-center">
                                <div class="form-inline">
                                    <div class="form-group" id="divSearch" runat="server">
                                        <label for="txtSerialNo" class="sr-only">Serial Number</label>
                                        <asp:TextBox ID="txtSerialNo" runat="server" CssClass="form-control input-lg" placeholder="Serial Number"></asp:TextBox>
                                    </div>
                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-lg btn-primary-outline"
                                        OnClick="btnSearch_Click">
                                    <span class="icon icon-magnifying-glass"></span> 
                                    Search
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Select Customer From Advance">
                    <div role="alert" runat="server" id="divAdvanceMsg">
                        <strong>No record found..</strong>
                    </div>
                    <ul class="list-group">
                        <asp:Repeater runat="server" ID="gvAdvanceCustomer">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbSelectAdvance" CssClass="list-group-item" OnClick="lbSelectAdvance_Click">
                                    <dl class="dl-horizontal">
                                        <asp:HiddenField ID="hfCustNo" runat="server" Value='<%# Bind("custNo") %>' />
                                        <asp:HiddenField ID="hfName" runat="server" Value='<%# Bind("Name") %>' />
                                        <asp:HiddenField ID="hfBRN" runat="server" Value='<%# Bind("BRN") %>' />
                                        <dt>Company Name</dt>
                                        <dd><%# Eval("Name") %></dd>
                                        <dt>Contract</dt>
                                        <dd><%# Eval("contractNo") %></dd>
                                        <dt>Model</dt>
                                        <dd><%# Eval("model") %></dd>
                                        <dt>Item Number</dt>
                                        <dd><%# Eval("itemNo") %></dd>
                                        <dt>BRN</dt>
                                        <dd><%# Eval("BRN") %></dd>
                                    </dl>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep3" runat="server" Title="Select Customer From CRM">
                    <div role="alert" runat="server" id="divCRMCustomer">
                        <strong>Customer not under users' territory..</strong>
                    </div>
                    <ul class="list-group">
                        <asp:Repeater runat="server" ID="gvCRMCustomer">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbSelectCRM" OnClick="lbSelectCRM_Click" CssClass="list-group-item">
                                    <dl class="dl-horizontal">
                                        <asp:HiddenField ID="hfName" runat="server" Value='<%# Bind("name") %>' />
                                        <asp:HiddenField ID="hfBrn" runat="server" Value='<%# Bind("brn") %>' />
                                        <asp:HiddenField ID="hfAdvanceCustNo" runat="server" Value='<%# Bind("AdvanceCustNo") %>' />
                                        <asp:HiddenField ID="hfAdvanceCustName" runat="server" Value='<%# Bind("AdvanceCustName") %>' />
                                        <asp:HiddenField ID="hfAdvanceBRN" runat="server" Value='<%# Bind("AdvanceBRN") %>' />
                                        <dt>Company Name</dt>
                                        <dd><%# Eval("name") %></dd>
                                        <dt>BRN</dt>
                                        <dd><%# Eval("brn") %></dd>
                                    </dl>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep4" runat="server" Title="MIF Information">
                    <asp:HiddenField ID="hfCustNo" runat="server" />
                    <asp:HiddenField ID="hfCustName" runat="server" />
                    <asp:HiddenField ID="hfBRN" runat="server" />
                    <asp:MultiView ID="mvResult" runat="server">
                        <asp:View ID="vMifInfo" runat="server">
                            <asp:Repeater runat="server" ID="gvMifInfo">
                                <ItemTemplate>
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h1><%# Eval("serialNo") %> <small runat="server" id="smCustName"><%# Eval("name") %></small>
                                            </h1>
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
                                                <asp:LinkButton ID="btnShowCust" runat="server" CssClass="btn btn-sm btn-primary-outline pull-right"
                                                    OnClick="btnShowCust_Click"><span class="icon icon-list"></span> Other Machines Under <%# Eval("name") %></asp:LinkButton>
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
                                            <div>
                                                <asp:Literal ID="ltServiceHScript" runat="server"></asp:Literal>
                                                <div id="serviceHChartDiv"></div>
                                            </div>
                                        </div>
                                        <ul class="list-group p-t-md">
                                            <asp:Repeater runat="server" ID="gvServiceH">
                                                <ItemTemplate>
                                                    <a class="list-group-item text text-lg" href='<%# "#" + Eval("serviceNo") %>' data-toggle="collapse"
                                                        aria-expanded="false" aria-controls='<%# Eval("serviceNo") %>'>
                                                        <span><%# Eval("serviceNo") %></span><small class="pull-right text-muted"><%# Eval("entryDate") %></small>
                                                    </a>
                                                    <div class="collapse" id='<%# Eval("serviceNo") %>'>
                                                        <div class="well">
                                                            <dl class="dl">
                                                                <dt>Service Type</dt>
                                                                <dd><span><%# Eval("typeOfService") %></span></dd>
                                                                <dt>Technician</dt>
                                                                <dd><span><%# Eval("techCode") %> - <%# Eval("techName") %> - <%# Eval("team") %></span>
                                                                </dd>
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
                                <div class="col-md-4">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h4>Toner Supply History</h4>
                                        </div>
                                        <div class="panel-body">
                                            <div>
                                                <asp:Literal ID="ltTonerHScript" runat="server"></asp:Literal>
                                                <div id="tonerHChartDiv"></div>
                                            </div>
                                        </div>
                                        <ul class="list-group p-t-md">
                                            <asp:Repeater runat="server" ID="gvTonerH">
                                                <ItemTemplate>
                                                    <li class="list-group-item">
                                                        <h4 class="list-group-item-heading">
                                                            <span><%# Eval("serviceNo") %></span><small class="pull-right text-muted"><%# Eval("invoiceDate") %></small>
                                                        </h4>
                                                        <div class="line-separator"></div>
                                                        <div class="list-group-item-text">
                                                            <dl class="dl">
                                                                <dt>Toner Delivered</dt>
                                                                <dd><span><%# Eval("toners") %></span></dd>
                                                            </dl>
                                                        </div>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h4>Meter Reading History</h4>
                                        </div>
                                        <div class="panel-body">
                                            <div>
                                                <asp:Literal ID="ltMeterHScript" runat="server"></asp:Literal>
                                                <div id="meterHChartDiv"></div>
                                            </div>
                                        </div>
                                        <ul class="list-group p-t-md">
                                            <asp:Repeater runat="server" ID="gvMeterH">
                                                <ItemTemplate>
                                                    <li class="list-group-item">
                                                        <h4 class="list-group-item-heading">
                                                            <span><%# Eval("invoiceNo") %></span><small class="pull-right text-muted"><%# Eval("invoiceDate") %></small>
                                                        </h4>
                                                        <div class="line-separator"></div>
                                                        <div class="list-group-item-text">
                                                            <dl class="dl-horizontal">
                                                                <dt>BW Copies</dt>
                                                                <dd><span><%# Eval("bw") %></span></dd>
                                                                <dt>COL Copies</dt>
                                                                <dd><span><%# Eval("col") %></span></dd>
                                                            </dl>
                                                        </div>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </asp:View>
                        <asp:View ID="vSerialList" runat="server">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h1><%= hfCustName.Value %></h1>
                                    <h4><%= hfCustNo.Value %></h4>
                                    <h4><%= brn %></h4>
                                </div>
                                <ul class="list-group">
                                    <asp:Repeater ID="gvSerialList" runat="server">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbSerial" runat="server" CssClass="list-group-item" OnClick="lbSerial_Click" Text='<%# Eval("serialNo") %>'></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </div>
                            <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-danger-outline" OnClick="btnBack_Click"><span class="icon icon-back"></span> Back</asp:LinkButton>
                        </asp:View>
                    </asp:MultiView>
                </asp:WizardStep>
            </WizardSteps>
            <StepNavigationTemplate>
                <asp:LinkButton ID="btnPrevious" runat="server" CssClass="btn btn-danger-outline"
                    CommandName="MovePrevious" CausesValidation="false">
                    <span class="icon icon-back"></span>
                    Back
                </asp:LinkButton>
            </StepNavigationTemplate>
            <FinishNavigationTemplate>
                <asp:LinkButton ID="btnFinish" runat="server" CssClass="btn btn-success-outline"
                    CommandName="MoveComplete" CausesValidation="false">
                    <span class="icon icon-home"></span>
                    Finish
                </asp:LinkButton>
            </FinishNavigationTemplate>
        </asp:Wizard>
    </div>
</asp:Content>
