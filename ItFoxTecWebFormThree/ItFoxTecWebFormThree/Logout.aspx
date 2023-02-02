<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="ItFoxTecWebFormThree.Logout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2><i>Your session have been ended.</i></h2>
            <br />
            <asp:HyperLink ID="lnkReLogin" NavigateUrl="~/Default.aspx" runat="server">Re-Login</asp:HyperLink>
        </div>
    </form>
</body>
</html>
