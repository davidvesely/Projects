<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_01.Cars.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cars search</title>
    <link href="Content/Site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label runat="server">Producer:</asp:Label>
            <br />
            <asp:DropDownList ID="DropDownListProducer" runat="server" AutoPostBack="True" />
            <br />
            <asp:Label runat="server">Model:</asp:Label>
            <br />
            <asp:DropDownList ID="DropDownListModel" runat="server" />
            <br />
            <asp:CheckBoxList ID="CheckBoxListExtras" runat="server" EnableViewState="True" />
            <br />
            <asp:Button runat="server" ID="ButtonSubmit" Text="Submit" OnClick="ButtonSubmit_OnClick"/>
            <br />
            <asp:Literal runat="server" ID="LiteralSelected" EnableViewState="false" />
        </div>
    </form>
</body>
</html>
