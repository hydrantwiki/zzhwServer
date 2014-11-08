using System;
using System.Web.UI.HtmlControls;

namespace HydrantWiki.Web
{
    public partial class Base : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public HtmlGenericControl Body
        {
            get
            {
                return this.myBody;
            }
        }
    }
}