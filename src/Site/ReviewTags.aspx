<%@ Page Title="" Language="C#" MasterPageFile="~/App.Master" AutoEventWireup="true" CodeBehind="ReviewTags.aspx.cs" Inherits="HydrantWiki.ReviewTags" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="http://code.jquery.com/jquery-1.9.1.min.js"></script>
    <script src="http://code.jquery.com/jquery-migrate-1.1.1.min.js"></script>
    <script src='http://api.tiles.mapbox.com/mapbox.js/v0.6.7/mapbox.js'></script>
    <link href='http://api.tiles.mapbox.com/mapbox.js/v0.6.7/mapbox.css' rel='stylesheet' /> 

    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCwnUWy9gT-OvVhBX0R4CrvU49GUA7TzQM&sensor=false">
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                Name="Telerik.Web.UI.Common.Core.js">
            </asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                Name="Telerik.Web.UI.Common.jQuery.js">
            </asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                Name="Telerik.Web.UI.Common.jQueryInclude.js">
            </asp:ScriptReference>
        </scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <ajaxsettings>
            <telerik:AjaxSetting AjaxControlID="btnGetTag">
                <updatedcontrols>
                    <telerik:AjaxUpdatedControl ControlID="hypHome" />
                    <telerik:AjaxUpdatedControl ControlID="lblTitle" />
                    <telerik:AjaxUpdatedControl ControlID="btnGetTag" />
                    <telerik:AjaxUpdatedControl ControlID="btnReject" />
                    <telerik:AjaxUpdatedControl ControlID="btnAccept" />
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" />
                    <telerik:AjaxUpdatedControl ControlID="mapBody" />
                </updatedcontrols>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnReject">
                <updatedcontrols>
                    <telerik:AjaxUpdatedControl ControlID="hypHome" />
                    <telerik:AjaxUpdatedControl ControlID="lblTitle" />
                    <telerik:AjaxUpdatedControl ControlID="btnGetTag" />
                    <telerik:AjaxUpdatedControl ControlID="btnReject" />
                    <telerik:AjaxUpdatedControl ControlID="btnAccept" />
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" />
                </updatedcontrols>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAccept">
                <updatedcontrols>
                    <telerik:AjaxUpdatedControl ControlID="hypHome" />
                    <telerik:AjaxUpdatedControl ControlID="lblTitle" />
                    <telerik:AjaxUpdatedControl ControlID="btnGetTag" />
                    <telerik:AjaxUpdatedControl ControlID="btnReject" />
                    <telerik:AjaxUpdatedControl ControlID="btnAccept" />
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" />
                </updatedcontrols>
            </telerik:AjaxSetting>
        </ajaxsettings>
    </telerik:RadAjaxManager>
    <div id="ReviewTagsLogo">
        <img src="http://images.hydrantwiki.com/web/Logo_half.png" alt="Logo"/>
    </div>
    <div id="ReviewTagsHeader">
        <asp:HyperLink class="HomeLink" ID="hypHome" runat="server" NavigateUrl="~/Home.aspx">Home</asp:HyperLink>
        <asp:Label ID="lblTitle" runat="server" Text="Review Tags" CssClass="lblMyTagsHeader"></asp:Label>
    </div> 
    <div id="ReviewTagsBody">
        <telerik:RadButton runat="server" Text="Get Next Tag" ID="btnGetTag" 
            onclick="btnGetTag_Click" ClientIDMode="Static"></telerik:RadButton>
        <telerik:RadButton ID="btnReject" runat="server" CssClass="btnReject" 
            onclick="btnReject_Click" Text="Reject" ClientIDMode="Static">
        </telerik:RadButton>
        <telerik:RadButton ID="btnAccept" runat="server" 
            CssClass="btnAccept" onclick="btnAccept_Click" Text="Accept" 
            ClientIDMode="Static">
        </telerik:RadButton>
        <asp:Panel runat="server" ID="pnlMain">
            <asp:HiddenField ID="hidNearbyPointsCSV" runat="server" ClientIDMode="Static" />
            <asp:Label runat="server" Text="User ID:" ID="lblUserID" CssClass="lblUserID"></asp:Label>
            <asp:Label runat="server" Text="Tag Date Time:" ID="lblTagDateTime" CssClass="lblTagDateTime"></asp:Label>
            
            <asp:HiddenField runat="server" ID="hidLatitude" ClientIDMode="Static"></asp:HiddenField>
            <asp:HiddenField runat="server" ID="hidLongitude" ClientIDMode="Static"></asp:HiddenField>

            <script type="text/javascript">
                var map;
                var layer;
                var markerLayer;

                var panorama;

                $(document).ready(function () {
                    
                });

                function refreshMap() {
                    var latitude = $('#hidLatitude').val();
                    var longitude = $('#hidLongitude').val();
                    var csv = $('#hidNearbyPointsCSV').val();

                    map = mapbox.map('mapBody');
                    layer = mapbox.layer().id('bengecko.map-vqbbmhmw');
                    map.addLayer(layer);

                    markerLayer = mapbox.markers.layer();
                    map.addLayer(markerLayer);

                    if (csv) {
                        markerLayer.csv(csv);
                    }

                    var newFeature = { geometry: { coordinates: [longitude, latitude] }, properties: {} };
                    markerLayer.add_feature(newFeature);

                    map.centerzoom({ lat: latitude, lon: longitude }, 20, false);

                    var newCenter = new google.maps.LatLng(latitude, longitude);

                    var panoramaOptions = {
                        position: newCenter,
                        pov: {
                            heading: 0,
                            pitch: 10
                        }
                    };
                    panorama = new google.maps.StreetViewPanorama(document.getElementById('panoBody'), panoramaOptions);
                    
                }
            </script>
            <div class="imageBody" id="imageBody">
                <asp:Image runat="server" ID="imgTagImage" CssClass="imgTagImage"></asp:Image>
            </div>
            <div class="nearHydrants" id="nearHydrants">
                <telerik:RadGrid runat=server AutoGenerateColumns="False" CellSpacing="0" 
                    GridLines="None" ID="gridNearby" Height="100%" Width="100%" 
                    ViewStateMode="Enabled">
                    <clientsettings>
                        <selecting allowrowselect="True" useclientselectcolumnonly="True" />
                        <scrolling allowscroll="True" usestaticheaders="True" />
                    </clientsettings>
                    <mastertableview datakeynames="HydrantGuid">
                        <commanditemsettings exporttopdftext="Export to PDF" />
                        <commanditemsettings exporttopdftext="Export to PDF" />
                        <commanditemsettings exporttopdftext="Export to PDF" />
                        <rowindicatorcolumn filtercontrolalttext="Filter RowIndicator column" 
                            visible="True">
                            <HeaderStyle Width="20px" />
                        </rowindicatorcolumn>
                        <expandcollapsecolumn filtercontrolalttext="Filter ExpandColumn column" 
                            visible="True">
                            <HeaderStyle Width="20px" />
                        </expandcollapsecolumn>
                        <Columns>
                            <telerik:GridClientSelectColumn FilterControlAltText="Filter colSelect column" 
                                HeaderText="Select" UniqueName="colSelect">
                                <HeaderStyle Width="40px" />
                                <ItemStyle Width="40px" />
                            </telerik:GridClientSelectColumn>
                            <telerik:GridBoundColumn FilterControlAltText="Filter column column" 
                                HeaderText="Distance" UniqueName="colDistance" DataField="DistanceInFeet" 
                                DataFormatString="{0}">
                                <HeaderStyle Width="60px" />
                                <ItemStyle Width="60px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridImageColumn FilterControlAltText="Filter column1 column" 
                                HeaderText="Image" ImageHeight="" ImageWidth="" UniqueName="colImage" 
                                DataImageUrlFields="HydrantImageUrl" DataImageUrlFormatString="{0}">
                                <HeaderStyle Width="100px" />
                                <ItemStyle Width="100px" />
                            </telerik:GridImageColumn>
                        </Columns>
                        <editformsettings>
                            <editcolumn filtercontrolalttext="Filter EditCommandColumn column">
                            </editcolumn>
                        </editformsettings>
                    </mastertableview>
                    <filtermenu enableimagesprites="False">
                    </filtermenu>
                </telerik:RadGrid>
            </div>
            <div class="mapBody" id="mapBody"></div>
            <div class="panoBody" id="panoBody"></div>
        </asp:Panel>
    </div>
</asp:Content>
