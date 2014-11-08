<%@ Page Title="" Language="C#" MasterPageFile="~/App.Master" AutoEventWireup="true" CodeBehind="Hydrants.aspx.cs" Inherits="HydrantWiki.Hydrants" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src='http://api.tiles.mapbox.com/mapbox.js/v0.6.7/mapbox.js'></script>
    <link href='http://api.tiles.mapbox.com/mapbox.js/v0.6.7/mapbox.css' rel='stylesheet' /> 
    <script type="text/javascript" src="http://code.jquery.com/jquery-1.8.2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        var map;
        var layerHeatMap;
        var layerBackground;
        var markerLayer;
        
        function onLoad() {
            setupMap();
        }

        function hydrantDetailsReceived(response) {
            $('#hydrantDetails').html(response);
        }

        function setupHydrantDetailsEvent(f) {
            var elem = mapbox.markers.simplestyle_factory(f);

            MM.addEvent(elem, 'click', function(e) {
                // clear the details box
                $('#hydrantDetails').empty();

                var postURL = "/Handlers/HydrantDetail.ashx?HydrantGuid=" + f.properties.id;

                $.ajax({
                    url: postURL,
                    beforeSend: function (xhr) {
                        xhr.overrideMimeType("text/html; charset=x-user-defined");
                    }
                }).done(hydrantDetailsReceived);

                // prevent this event from bubbling down to the map and clearing
                // the content
                e.stopPropagation();
            });
            return elem;
        }

        function dataReceived(data) {
            if (markerLayer) {
                markerLayer.features([]);
                map.removeLayer(markerLayer);
            }
                    
            markerLayer = mapbox.markers.layer().csv(data);

            // Add interaction to this marker layer. This
            // binds tooltips to each marker that has title
            // and description defined.
            mapbox.markers.interaction(markerLayer);

            markerLayer.factory(setupHydrantDetailsEvent);

            map.addLayer(markerLayer);
            //map.refresh();       
        }

        function mapRefresh() {
            var zoomLevel = map.zoom();

            if (zoomLevel > 12) {
                var currentExtent = map.getExtent();

                var postURL = "/Handlers/HydrantQuery.ashx?east=" + currentExtent.east + "&west=" + currentExtent.west + "&north=" + currentExtent.north + "&south=" + currentExtent.south;

                $.ajax({
                    url: postURL,
                    beforeSend: function (xhr) {
                        xhr.overrideMimeType("text/plain; charset=x-user-defined");
                    }
                }).done(dataReceived);
            }
            else {
                if (markerLayer) {
                    markerLayer.features([]);
                    map.removeLayer(markerLayer);
                    map.refresh();
                }
            }
        }

        function zoomedCallback(map, zoomOffset) {
            mapRefresh();
        }

        function pannedCallback(map, panOffset) {
            mapRefresh();
        }

        function setupMap() {
            map = mapbox.map('hydrantsMap');
            map.ui.zoomer.add();
            map.ui.zoombox.add();

            layerBackground = mapbox.layer().id('bengecko.map-vqbbmhmw');
            map.addLayer(layerBackground);

            layerHeatMap = mapbox.layer().id('bengecko.HydrantWikiHeatMap');
            map.addLayer(layerHeatMap);

            map.centerzoom({ lat: 40.26, lon: -96.870 }, 3, false);

            map.addCallback("zoomed", zoomedCallback);
            map.addCallback("panned", pannedCallback);

            MM.addEvent(map.parent, 'click', function () {
                $('#hydrantDetails').empty();
            });

        }
    </script>
    <div class="homeHeader">
        <img src="http://images.hydrantwiki.com/web/Logo_half.png" />
    </div>
    <div id="MyTagsHeader">
        <asp:HyperLink class="HomeLink" ID="hypHome" runat="server" NavigateUrl="~/Home.aspx">Home</asp:HyperLink>
        <asp:Label ID="lblTitle" runat="server" Text="Hydrants" CssClass="lblMyTagsHeader"></asp:Label>
    </div> 
    <div class="hydrantsMap" id="hydrantsMap"> 
    </div>
    <div class="hydrantDetails" id="hydrantDetails">
    </div>
</asp:Content>
