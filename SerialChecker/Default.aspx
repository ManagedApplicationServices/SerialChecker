<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SerialChecker._Default" EnableEventValidation="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        #wizHeader li :first-of-type {
            border-bottom-left-radius: 4px;
            border-top-left-radius: 4px;
        }

        #wizHeader li .prevStep {
            background-color: #355264;
        }

            #wizHeader li .prevStep:after {
                border-left-color: #355264 !important;
            }

        #wizHeader li .currentStep {
            background-color: #253b4a;
        }

            #wizHeader li .currentStep:after {
                border-left-color: #253b4a !important;
            }

        #wizHeader li .nextStep {
            background-color: #C2C2C2;
        }

            #wizHeader li .nextStep:after {
                border-left-color: #C2C2C2 !important;
            }

        #wizHeader {
            list-style: none;
            overflow: hidden;
            margin: 0px;
            padding: 0px;
        }

            #wizHeader li {
                float: left;
            }

                #wizHeader li a {
                    color: white;
                    text-decoration: none;
                    padding: 10px 0 10px 55px;
                    background: brown;
                    background: hsla(34,85%,35%,1);
                    position: relative;
                    display: block;
                    float: left;
                }

                    #wizHeader li a:after {
                        content: " ";
                        display: block;
                        width: 0;
                        height: 0;
                        border-top: 50px solid transparent;
                        border-bottom: 50px solid transparent;
                        border-left: 30px solid hsla(34,85%,35%,1);
                        position: absolute;
                        top: 50%;
                        margin-top: -50px;
                        left: 100%;
                        z-index: 2;
                    }

                    #wizHeader li a:before {
                        content: " ";
                        display: block;
                        width: 0;
                        height: 0;
                        border-top: 50px solid transparent;
                        border-bottom: 50px solid transparent;
                        border-left: 30px solid white;
                        position: absolute;
                        top: 50%;
                        margin-top: -50px;
                        margin-left: 1px;
                        left: 100%;
                        z-index: 1;
                    }

                #wizHeader li:first-child a {
                    padding-left: 10px;
                }

                #wizHeader li:last-child {
                    padding-right: 50px;
                }
        /*
                #wizHeader li a:hover {
                    background: #FE9400;
                }

                    #wizHeader li a:hover:after {
                        border-left-color: #FE9400 !important;
                    }
                    */
        /*            
        .WizardStep.ActiveStep {
            background: #253b4a;
        }

        .WizardParent {
            border-radius: 4px;
        }

        .WizardStep.ActiveStep a,
        .WizardStep.ActiveStep a:link {
            color: #fff;
        }

        .WizardStep.Past {
            background: #355264;
        }

            .WizardStep.Past a,
            .WizardStep.Past a:link {
                color: #fff;
            }

        .WizardStep a,
        .WizardStep a:link {
            text-decoration: none;
        }

            .WizardStep a,
            .WizardStep a:link,
            .WizardStep a[disabled="disabled"],
            .WizardStep a[disabled="disabled"]:hover {
                color: #333;
            }

        .WizardStep:first-of-type {
            border-bottom-left-radius: 4px;
            border-top-left-radius: 4px;
        }

        .WizardStep:last-of-type {
            border-bottom-right-radius: 4px;
            border-top-right-radius: 4px;
        }

        .WizardStep.Past:before {
            border-bottom-color: #355264;
            border-top-color: #355264;
        }

        .WizardStep.ActiveStep:before {
            border-bottom-color: #253b4a;
            border-top-color: #253b4a;
        }
        .content {
            height: 150px;
            padding-top: 75px;
            text-align: center;
            background-color: #F9F9F9;
            font-size: 48px;
        }
           */
    </style>
    <script type="text/javascript">

        function AdvanceSelect(id) {
            document.getElementById('MainContent_Wizard1_gvAdvanceCustomer_' + id).click();
        }
        function CRMSelect(id) {
            document.getElementById('MainContent_Wizard1_gvCRMCustomer_' + id).click();
        }
    </script>
    <%--<div class="page-header">
        <h1>Search for serial status</h1>
    </div>--%>
    <div class="page-header" style="border-bottom: none; font-size: 15px;">
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
                    <div class="well well-lg" align="center" >
                        <div class="form-inline">
                            <div class="form-group" id="divSearch" runat="server">
                                <label for="txtSerialNo" class="sr-only">Serial Number</label>
                                <asp:TextBox ID="txtSerialNo" runat="server" CssClass="form-control input-lg" placeholder="Serial Number"></asp:TextBox>
                            </div>
                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success btn-lg" OnClick="btnSearch_Click" />
                        </div>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Select Customer From Advance">
                    <div class="container-fluid">
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
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep3" runat="server" Title="Selet Customer From CRM">
                    <div class="container-fluid">
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
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep4" runat="server" Title="MIF Information">
                    <div class="container-fluid">
                        <asp:GridView ID="gvMifInfo" runat="server" CssClass="table table-bordered" EmptyDataText="Customer not found."
                            AutoGenerateColumns="false" ShowHeader="false">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <div class="container-fluid">
                                            <div class="page-header">
                                                <div class="col-sm-12">
                                                    <h1><%# Eval("serialNo") %> <small runat="server"><%# Eval("name") %></small></h1>
                                                </div>
                                            </div>
                                            <div class="col-sm-12">
                                                <div class="col-sm-2">
                                                    <b>Model: </b>
                                                </div>
                                                <div class="col-sm-10">
                                                    <asp:Label ID="lblModel" runat="server" Text='<%# Bind("model") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-2">
                                                    <b>Contract: </b>
                                                </div>
                                                <div class="col-sm-10">
                                                    <asp:Label ID="lblCntrctNo" runat="server" Text='<%# Bind("contractNo") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-2">
                                                    <b>Contract End: </b>
                                                </div>
                                                <div class="col-sm-10">
                                                    <asp:Label ID="lblCntrctEnd" runat="server" Text='<%# Bind("cntrctEnd") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-2">
                                                    <b>Contact: </b>
                                                </div>
                                                <div class="col-sm-10">
                                                    <asp:Label ID="lblContact" runat="server" Text='<%# Bind("contact") %>'></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="col-sm-12">
                                                <div class="col-sm-2">
                                                    <b>Location: </b>
                                                </div>
                                                <div class="col-sm-10">
                                                    <asp:Label ID="lblLocation" runat="server" Text='<%# Bind("location") %>'></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:WizardStep>
            </WizardSteps>
            <StepNavigationTemplate>
                <asp:Button ID="btnPrevious" runat="server" Text="<< Previous" CssClass="btn btn-warning" CommandName="MovePrevious" CausesValidation="false" />
            </StepNavigationTemplate>
            <FinishNavigationTemplate>
                <asp:Button ID="btnFinish" runat="server" Text="Finish" CssClass="btn btn-success" CommandName="MoveComplete" CausesValidation="false" />
            </FinishNavigationTemplate>
        </asp:Wizard>
    </div>
</asp:Content>
