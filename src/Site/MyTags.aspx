<%@ Page Title="" Language="C#" MasterPageFile="~/App.Master" AutoEventWireup="true" CodeBehind="MyTags.aspx.cs" Inherits="HydrantWiki.MyTags" %>
<%@ Register assembly="RadGrid.Net2" namespace="Telerik.WebControls" tagprefix="rad" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src='http://api.tiles.mapbox.com/mapbox.js/v0.6.7/mapbox.js'></script>
    <link href='http://api.tiles.mapbox.com/mapbox.js/v0.6.7/mapbox.css' rel='stylesheet' /> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
    </telerik:RadStyleSheetManager>
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
            </asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
            </asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
            </asp:ScriptReference>
        </scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <ajaxsettings>
            <telerik:AjaxSetting AjaxControlID="btnFirstPage">
                <updatedcontrols>
                    <telerik:AjaxUpdatedControl ControlID="gridTags" />
                </updatedcontrols>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnNextPage">
                <updatedcontrols>
                    <telerik:AjaxUpdatedControl ControlID="gridTags" />
                </updatedcontrols>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnDelete">
                <updatedcontrols>
                    <telerik:AjaxUpdatedControl ControlID="gridTags" />
                </updatedcontrols>
            </telerik:AjaxSetting>
        </ajaxsettings>
    </telerik:RadAjaxManager>
    <script type="text/javascript" >
        var map;
        var layer;
        var markerLayer;

        function setupMap() {
            map = mapbox.map('MyTagsMap');
            layer = mapbox.layer().id('bengecko.map-vqbbmhmw');
            map.addLayer(layer);

            markerLayer = mapbox.markers.layer();
            map.addLayer(markerLayer);

            map.centerzoom({ lat: 40.26, lon: -96.870 }, 3);
            map.ui.attribution.add().content('<a href="http://mapbox.com/about/maps">Terms &amp; Feedback</a>');
        }

        function RowSelected(sender, eventArgs) {
            var latitude = eventArgs.get_item().get_cell("colLatitude").innerHTML;
            var longitude = eventArgs.get_item().get_cell("colLongitude").innerHTML;

            markerLayer.features(null);

            var newFeature = { geometry: { coordinates: [longitude, latitude] }, properties: {} };
            markerLayer.add_feature(newFeature);

            map.centerzoom({ lat: latitude, lon: longitude }, 16)
        }

    </script>
    <div id="ReviewTagsLogo">
        <img src="http://images.hydrantwiki.com/web/Logo_half.png" />
    </div>
    <div id="MyTagsHeader">
        <asp:HyperLink class="HomeLink" ID="hypHome" runat="server" NavigateUrl="~/Home.aspx">Home</asp:HyperLink>
        <asp:Label ID="lblTitle" runat="server" Text="My Tags" CssClass="lblMyTagsHeader"></asp:Label>
    </div> 
    <div id="MyTagsButtonBar">
        <div class="btnFirstPage">
            <telerik:RadButton ID="btnFirstPage" runat="server" Text="First Page" onclick="btnFirstPage_Click" >
            </telerik:RadButton>
        </div>
        <div class="btnNextPage">
            <telerik:RadButton ID="btnNextPage" runat="server" Text="Next Page" onclick="btnNextPage_Click">
            </telerik:RadButton>
        </div>
        <div class="btnDelete">
            <telerik:radbutton runat="server" text="Delete Tag" ID="btnDelete" onclick="btnDelete_Click" >
            </telerik:radbutton>
        </div>
    </div>
    <div id="MyTagsTable">
        <telerik:RadGrid ID="gridTags" runat="server" AutoGenerateColumns="False" 
            CellSpacing="0" GridLines="None" 
            onneeddatasource="gridTags_NeedDataSource" style="margin-right: 10px">
            <clientsettings>
                <scrolling allowscroll="True" scrollheight="400px" usestaticheaders="True" />
                <selecting allowrowselect="True" />
                <clientevents onrowselected="RowSelected" />
                <selecting allowrowselect="True" useclientselectcolumnonly="True" />

<Selecting AllowRowSelect="True" enabledragtoselectrows="False"></Selecting>

<ClientEvents OnRowSelected="RowSelected"></ClientEvents>

<Scrolling AllowScroll="True" ScrollHeight="400px" UseStaticHeaders="True"></Scrolling>
            </clientsettings>
            <MasterTableView grouploadmode="Client" datakeynames="TagGuid">
                <CommandItemSettings ExportToPdfText="Export to PDF">
                </CommandItemSettings>
                <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </RowIndicatorColumn>
                <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </ExpandCollapseColumn>
                <Columns>
                    <telerik:GridClientSelectColumn FilterControlAltText="Filter colSelect column" 
                        UniqueName="colSelect">
                    </telerik:GridClientSelectColumn>
                    <telerik:GridBoundColumn DataField="DeviceDateTime" 
                        FilterControlAltText="Filter colDateTimeStamp column" 
                        HeaderText="Tag Date Time" UniqueName="colDateTimeStamp">
                        <FooterStyle Width="120px" />
                        <HeaderStyle Width="120px" />
                        <ItemStyle Width="120px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridNumericColumn DataField="Latitude" DecimalDigits="6" 
                        FilterControlAltText="Filter colLatitude column" UniqueName="colLatitude" 
                        DataType="System.Double" HeaderText="Latitude">
                        <FooterStyle Width="100px" />
                        <HeaderStyle Width="100px" />
                        <ItemStyle Width="100px" />
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="Longitude" DecimalDigits="6" 
                        FilterControlAltText="Filter colLongitude column" 
                        UniqueName="colLongitude" DataType="System.Double" 
                        HeaderText="Longitude">
                        <FooterStyle Width="100px" />
                        <HeaderStyle Width="100px" />
                        <ItemStyle Width="100px" />
                    </telerik:GridNumericColumn>
                    <telerik:GridImageColumn DataImageUrlFields="ThumbImageUrl" 
                        DataImageUrlFormatString="{0}" FilterControlAltText="Filter colImage column" 
                        HeaderText="Tag Image" ImageHeight="" ImageWidth="" UniqueName="colImage">
                        <FooterStyle Width="100px" />
                        <HeaderStyle Width="100px" />
                        <ItemStyle Width="100px" />
                    </telerik:GridImageColumn>
                    <telerik:GridBoundColumn DataField="TagGuid" 
                        FilterControlAltText="Filter column column" HeaderText="TagGuid" 
                        UniqueName="column" Visible="False">
                    </telerik:GridBoundColumn>
                </Columns>
                <EditFormSettings>
                    <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                    </EditColumn>
                </EditFormSettings>
            </MasterTableView>
            <FilterMenu EnableImageSprites="False">
            </FilterMenu>
        </telerik:RadGrid>
    </div>
    <div id="MyTagsMap" >
    </div>
</asp:Content>
