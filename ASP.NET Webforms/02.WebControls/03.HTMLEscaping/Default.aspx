<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
     Inherits="_03.HTMLEscaping.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HTML Escaping</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:TextBox ID="TextBoxSampleText" runat="server" Width="330px">&lt;h1&gt;Heading&lt;/h1&gt;Text&lt;script&gt;alert(&quot;bug!&quot;)&lt;/script&gt;</asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonJustShowText" runat="server" Text="Show text"
          OnClick="ButtonJustShowText_Click" />
        &nbsp;&nbsp; 
        <asp:Button ID="ButtonShowHtmlEncoded" runat="server" Text="Show text (HTML encoded)"
          OnClick="ButtonShowHtmlEncoded_Click" />    
        <br />
        <br />
        <strong>Label (unescaped):</strong>
        <br />
        <asp:Label ID="LabelEnteredText" runat="server" />
        <hr />
        <br />
        <strong>HTML escaped literal:</strong>
        <br />
        <asp:Literal ID="LiteralEnteredText" runat="server" Mode="Encode" />
        <hr />
        <br />
        <strong>&lt;%: some text %&gt;:</strong>
        <br />
        <%: this.TextBoxSampleText.Text %>
    </form>
</body>
</html>
