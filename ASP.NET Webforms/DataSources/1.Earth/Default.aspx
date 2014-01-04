<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_1.Earth.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Earth model</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:EntityDataSource ID="EntityDataSourceContinents" runat="server" ConnectionString="name=EarthDBEntities"
                DefaultContainerName="EarthDBEntities" EnableFlattening="False" EntitySetName="Continents" />

            <asp:ListBox runat="server" ID="ListBoxContinents" DataSourceID="EntityDataSourceContinents"
                ItemType="_1.Earth.Continent" DataValueField="ID" DataTextField="Name"
                Rows="7" AutoPostBack="True" />
        </div>

        <div>
            <asp:EntityDataSource runat="server" ID="EntityDataSourceCountries" ConnectionString="name=EarthDBEntities"
                DefaultContainerName="EarthDBEntities" EnableFlattening="False" EntitySetName="Countries"
                Where="it.ContinentID=@ContinentID">
                <WhereParameters>
                    <asp:ControlParameter Name="ContinentID" Type="Int32" ControlID="ListBoxContinents" />
                </WhereParameters>
            </asp:EntityDataSource>

            <asp:GridView runat="server" ID="GridViewCountries" DataSourceID="EntityDataSourceCountries"
                AutoGenerateColumns="False" DataKeyNames="ID" AllowPaging="True" AllowSorting="True" PageSize="3">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                    <asp:BoundField DataField="Population" HeaderText="Population" SortExpression="Population" />
                    <asp:BoundField DataField="Language" HeaderText="Language" SortExpression="Language" />
                </Columns>
            </asp:GridView>
        </div>
        
        <asp:EntityDataSource runat="server" ID="EntityDataSourceTowns" ConnectionString="name=EarthDBEntities"
            DefaultContainerName="EarthDBEntities" EnableFlattening="False" EntitySetName="Towns"
            Where="it.CountryID=@CountryID">
            <WhereParameters>
                <asp:ControlParameter Name="CountryID" Type="Int32" ControlID="GridViewCountries" />
            </WhereParameters>
        </asp:EntityDataSource>
        
        <asp:ListView runat="server" ID="ListViewTowns" DataSourceID="EntityDataSourceTowns"
            ItemType="_1.Earth.Town">
            <ItemTemplate>
                <div style="padding: 5px; float: left">
                    Town Name: <%#: Item.Name %><br />
                    Population: <%#: Item.Population %><br />
                </div>
            </ItemTemplate>
        </asp:ListView>
    </form>
</body>
</html>
