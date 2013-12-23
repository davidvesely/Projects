<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_01.RandomNumbers.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Random numbers</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label runat="server">Range start:</asp:Label>
            <asp:TextBox runat="server" ID="TextBoxRangeStart"/>
        </div>
        <div>
            <asp:Label runat="server">Range end:</asp:Label>
            <asp:TextBox runat="server" ID="TextBoxRangeEnd"/>
        </div>
        <asp:Button runat="server" ID="ButtonGenerate" Text="Generate Random" OnClick="ButtonGenerate_OnClick"/>
        <div>
            <asp:Label runat="server" ID="LabelResult"/>
        </div>
    </form>
</body>
</html>
