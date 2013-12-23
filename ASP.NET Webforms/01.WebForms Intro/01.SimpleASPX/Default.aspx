<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_01.SimpleASPX.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label runat="server" ID="LabelName">Name:</asp:Label>
            <asp:TextBox runat="server" ID="TextBoxName" />
            <asp:Button runat="server" ID="ButtonShowGreeting" OnClick="ButtonShowGreeting_OnClick" Text="Show Greeting"/>
        </div>
        <div>
            <asp:Label runat="server" ID="LabelGreeting" />
            <br />
            <asp:Label runat="server" ID="LabelAssembly" />
        </div>
    </form>
</body>
</html>
