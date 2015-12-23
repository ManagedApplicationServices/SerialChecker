<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ErrorPage.aspx.cs" Inherits="SerialChecker.ErrorPage" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <div class="container">
            <h1>Oops, something went wrong.</h1>
            <p>Please <a href="http://rsphelpdesk:8088/">log a ticket</a> to MAS to report the issue.
            </p>
            <p><a class="btn btn-primary btn-lg" href="Default.aspx" role="button"><span class="icon icon-back">
            </span>Back to home</a></p>
        </div>
    </div>
</asp:Content>
