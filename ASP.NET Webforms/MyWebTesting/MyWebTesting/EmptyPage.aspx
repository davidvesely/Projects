<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmptyPage.aspx.cs" Inherits="MyWebTesting.EmptyPage" %>

<%@ Register Src="~/SampleInput.ascx" TagPrefix="uc1" TagName="SampleInput" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <uc1:SampleInput runat="server" id="SampleInput" />
    </form>
</body>
</html>
