<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebsiteTest.About"
    culture="bg-BG" meta:resourcekey="PageResource1" uiculture="bg-BG" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Your app description page.</h2>
    </hgroup>

    <article>
        <p>        
            Use this area to provide additional information.
        </p>

        <p>        
            Use this area to provide additional information.
        </p>

        <p>        
            Use this area to provide additional information.
        </p>

        <dx:ASPxGridView ID="ASPxGridView1" runat="server">
            <Columns>
                <dx:GridViewDataTextColumn Caption="ID" FieldName="ID" />
                <dx:GridViewDataTextColumn Caption="Name" FieldName="Name" />
            </Columns>
        </dx:ASPxGridView>
    </article>

    <aside>
        <h3>Aside Title</h3>
        <p>        
            Use this area to provide additional information.
        </p>
        <ul>
            <li><a runat="server" href="~/">Home</a></li>
            <li><a runat="server" href="~/About">About</a></li>
            <li><a runat="server" href="~/Contact">Contact</a></li>
        </ul>
    </aside>

    <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="English label" meta:resourcekey="ASPxLabel1Resource1"></dx:ASPxLabel>
    <br />
    <dx:ASPxButton ID="ASPxButton1" runat="server" Text="A Button" meta:resourcekey="ASPxButton1Resource1"></dx:ASPxButton>
    <br />
    <asp:Literal ID="Literal1" runat="server"></asp:Literal>
</asp:Content>