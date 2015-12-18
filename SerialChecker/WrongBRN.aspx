<%@ Page Title="WrongBRN" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WrongBRN.aspx.cs" Inherits="SerialChecker.WrongBRN" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="panel panel-default">
        <div class="panel-heading">
            <h1><%= Request.QueryString["name"] %></h1> 
        </div>
        <div class="panel-body">
            <div role="alert" runat="server" id="divMsg" class="hidden">
                <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <strong>Record submitted</strong>
            </div>
            <div class="form-inline">
                <div class="form-group" id="divBrn" runat="server">
                    <label for="txtBrn" class="sr-only">Enter the correct BRN</label>
                    <asp:TextBox ID="txtBrn" runat="server" CssClass="form-control input-lg" placeholder="Enter the correct BRN"></asp:TextBox>
                </div>
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-lg btn-primary" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
</asp:Content>
