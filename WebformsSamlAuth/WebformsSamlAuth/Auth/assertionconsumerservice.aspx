<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="assertionconsumerservice.aspx.cs" Inherits="WebformsSamlAuth.Auth.assertionconsumerservice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Assertion Consumer Service</h2>
            <br />
            <asp:Label ID="LabelSuccess" runat="server" ForeColor="#00F700"></asp:Label>
            <br />
            <asp:Label ID="LabelErrorMessage" runat="server" ForeColor="Red"></asp:Label>
        </div>
    </form>
</body>
</html>
