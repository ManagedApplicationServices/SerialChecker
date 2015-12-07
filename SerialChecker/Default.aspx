<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SerialChecker._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1>Search for serial status</h1>
    </div>
    <div class="form-inline">
        <div class="form-group">
            <label for="txtSerialNo" class="sr-only">Serial Number</label>
            <asp:TextBox ID="txtSerialNo" runat="server" CssClass="form-control" placeholder="Serial Number"></asp:TextBox>
        </div>
        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success" OnClick="btnSearch_Click" />
    </div>
    <div class="container-fluid" style="margin-top: 10px">
        <asp:GridView ID="gvSearchResult" runat="server" CssClass="table table-bordered" AutoGenerateColumns="false" ShowHeader="false" EmptyDataText="No item to show..">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <div>
                            <div class="col-sm-12">
                                <div class="col-sm-2">
                                    <b >Company Name: </b>
                                </div>
                                <div class="col-sm-10">
                                    <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="col-sm-12">
                                <div class="col-sm-2">
                                    <b>Contract: </b>
                                </div>
                                <div class="col-sm-10">
                                    <asp:Label ID="lblContract" runat="server" Text='<%# Bind("contractNo") %>'></asp:Label>
                                </div>
                            </div>
                            <br />
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
                                    <b>Item Number: </b>
                                </div>
                                <div class="col-sm-10">
                                    <asp:Label ID="lblItemNo" runat="server" Text='<%# Bind("itemNo") %>'></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="col-sm-12">
                                <div class="col-sm-2">
                                    <b>BRN: </b>
                                </div>
                                <div class="col-sm-10">
                                    <asp:Label ID="lblBRN" runat="server" Text='<%# Bind("BRN") %>'></asp:Label>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
