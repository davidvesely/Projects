<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_01.Cars.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cars search</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label runat="server">Producer:</asp:Label>
        <asp:DropDownList ID="DropDownListProducer" runat="server" AutoPostBack="True"></asp:DropDownList>
        <br />
        <asp:Label runat="server">Model:</asp:Label>
        <asp:DropDownList ID="DropDownListModel" runat="server"></asp:DropDownList>
        <br />
        <asp:Literal runat="server" ID="LiteralSelected" EnableViewState="false"/>
    </form>
</body>
</html>
