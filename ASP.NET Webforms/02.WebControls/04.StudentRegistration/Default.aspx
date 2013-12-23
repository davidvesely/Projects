<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_04.StudentRegistration.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Student Registration</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label runat="server">First name:</asp:Label>
            <asp:TextBox runat="server" ID="TextBoxFirstName" />
        </div>
        <div>
            <asp:Label runat="server">Second name:</asp:Label>
            <asp:TextBox runat="server" ID="TextBoxSecondName" />
        </div>
        <div>
            <asp:Label runat="server">Faculty number:</asp:Label>
            <asp:TextBox runat="server" ID="TextBoxFacultyNumber" />
        </div>
        <div>
            <asp:Label runat="server">University</asp:Label>
            <asp:DropDownList ID="DropDownListUniversity" runat="server">
                <asp:ListItem Value="1">TU-Sofia</asp:ListItem>
                <asp:ListItem Value="2">Sofia University</asp:ListItem>
                <asp:ListItem Value="3">NBU</asp:ListItem>
                <asp:ListItem Value="4">Software University</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div>
            <asp:Label runat="server">Speciality</asp:Label><br/>
            <asp:ListBox ID="ListBoxSpeciality" SelectionMode="Multiple" runat="server">
                <asp:ListItem Value="1">Spec1</asp:ListItem>
                <asp:ListItem Value="2">Spec2</asp:ListItem>
                <asp:ListItem Value="3">Spec3</asp:ListItem>
                <asp:ListItem Value="4">Spec4</asp:ListItem>
                <asp:ListItem Value="5">Spec5</asp:ListItem>
                <asp:ListItem Value="6">Spec6</asp:ListItem>
                <asp:ListItem Value="7">Spec7</asp:ListItem>
                <asp:ListItem Value="8">Spec8</asp:ListItem>
                <asp:ListItem Value="9">Spec9</asp:ListItem>
                <asp:ListItem Value="10">Spec10</asp:ListItem>
            </asp:ListBox>
        </div>
        <asp:Button runat="server" ID="ButtonRegister" Text="Register"/>
        <br />
        <asp:Literal runat="server" Mode="Encode" ID="LiteralStudent"/>
        <br /><br />
        <% if (Page.IsPostBack) { %>
            Student name: <%: TextBoxFirstName.Text + " " + TextBoxSecondName.Text %><br />
            Faculty number: <%: TextBoxFacultyNumber.Text %><br />
            University: <%: DropDownListUniversity.SelectedValue %><br />
            Speciality: <%: _specialities.ToString() %><br />
        <% } %>
    </form>
</body>
</html>
