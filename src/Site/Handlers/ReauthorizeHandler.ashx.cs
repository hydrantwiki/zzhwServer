using System.Web;
using System.Web.SessionState;
using HydrantWiki.Library.Managers;
using TreeGecko.Library.Net.Handlers;
using TreeGecko.Library.Net.Helpers;

namespace HydrantWiki.Web.Handlers
{
    /// <summary>
    /// Summary description for ReauthorizeHandler
    /// </summary>
    public class ReauthorizeHandler : AbstractHandler, IRequiresSessionState
    {
        public override void HandleGet(HttpContext _context)
        {
            _context.Response.ContentType = "application/json";

            HydrantWikiManager hwManager = new HydrantWikiManager();

            if (LoginHelper.LoginAndCreateSession(hwManager, _context))
            {
                _context.Response.Write(@" { ""Result"":""Success"" } ");
            }
            else
            {
                _context.Response.Write(@" { ""Result"":""Failure"" } ");
            }
        }
    }
}