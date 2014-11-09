<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SampleInput.ascx.cs" Inherits="MyWebTesting.SampleInput" %>
<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<script type="text/javascript">
    function validateInputs(s, e) {
        console.log(e);
    }
</script>
<table>
    <tr>
        <td>
            <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="Input 1"></dx:ASPxLabel>
        </td>
        <td>
            <dx:ASPxTextBox ID="ASPxTextBox1" runat="server" Width="170px" ClientInstanceName="textBox1">
                <ClientSideEvents Validation="validateInputs" />
                <ValidationSettings ValidationGroup="vg1" />
            </dx:ASPxTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <dx:ASPxLabel ID="ASPxLabel2" runat="server" Text="Input 2"></dx:ASPxLabel>
        </td>
        <td>
            <dx:ASPxTextBox ID="ASPxTextBox2" runat="server" Width="170px" ClientInstanceName="textBox2">
                <ClientSideEvents Validation="validateInputs" />
                <ValidationSettings ValidationGroup="vg1" />
            </dx:ASPxTextBox>
        </td>
    </tr>
</table>

<dx:ASPxButton ID="ASPxButton1" runat="server" Text="Submit" CausesValidation="true" />
