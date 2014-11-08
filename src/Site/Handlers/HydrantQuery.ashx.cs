using System;
using System.Collections.Generic;
using System.Web;
using HydrantWiki.Library.Helpers;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using TreeGecko.Library.Geospatial.Objects;
using TreeGecko.Library.Net.Handlers;
using TreeGecko.Library.Net.Helpers;

namespace HydrantWiki.Web.Handlers
{
    /// <summary>
    /// Summary description for HydrantQuery
    /// </summary>
    public class HydrantQuery : AbstractHandler, System.Web.SessionState.IRequiresSessionState
    {
        public override void HandleGet(HttpContext _context)
        {
            HydrantWikiManager hwManager = new HydrantWikiManager();

            if (LoginHelper.LoginAndCreateSession(hwManager, _context))
            {
                HttpRequest request = _context.Request;

                string sEast = request.QueryString["east"];
                string sWest = request.QueryString["west"];
                string sNorth = request.QueryString["north"];
                string sSouth = request.QueryString["south"];

                GeoBox gb = new GeoBox(Convert.ToDouble(sEast),
                                       Convert.ToDouble(sWest),
                                       Convert.ToDouble(sNorth),
                                       Convert.ToDouble(sSouth));

                List<HydrantPosition> positions = hwManager.GetHydrantPositions(gb);

                _context.Response.ContentType = "text/plain";
                _context.Response.Write(HydrantCSVHelper.GetHydrantCSV(positions));
            }
        }
    }
}