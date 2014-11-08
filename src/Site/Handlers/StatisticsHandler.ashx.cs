using System.Text;
using System.Web;
using System.Web.SessionState;
using HydrantWiki.Library.Managers;
using TreeGecko.Library.Net.Handlers;
using TreeGecko.Library.Net.Helpers;

namespace HydrantWiki.Web.Handlers
{
    /// <summary>
    /// Summary description for StatisticsHandler
    /// </summary>
    public class StatisticsHandler : AbstractHandler, IRequiresSessionState
    {
        public override void HandleGet(HttpContext _context)
        {
            _context.Response.ContentType = "application/json";

            HydrantWikiManager hwManager = new HydrantWikiManager();

            if (LoginHelper.LoginAndCreateSession(hwManager, _context))
            {
                long siteUserCount = hwManager.GetUserCount();
                long siteHydrantCount = hwManager.GetHydrantCount();
                long siteTagCount = hwManager.GetTagCount();

                StringBuilder sb = new StringBuilder();
                sb.Append(@"{ ""Result"":""Success"", ""SiteUserCount"":""");
                sb.Append(siteUserCount);
                sb.Append(@""", ""SiteHydrantCount"":""");
                sb.Append(siteHydrantCount);
                sb.Append(@""", ""SiteTagCount"":""");
                sb.Append(siteTagCount);
                sb.Append(@""" }");

                string temp = sb.ToString();

                _context.Response.Write(temp);
            }
            else
            {
                _context.Response.Write(@" { ""Result"":""Failure"" }");
            }
        }
    }
}