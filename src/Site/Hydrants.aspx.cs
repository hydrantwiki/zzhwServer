using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCN.Library.Objects;
using System.Web.UI.HtmlControls;

namespace HydrantWiki
{
    public partial class Hydrants : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User user = (User)Session["User"];

            if (user == null)
            {
                Response.Redirect("/default.aspx");
            }

            if (!IsPostBack)
            {
                HtmlGenericControl mybody = (this.Master as HydrantWiki.App.App).Body;
                mybody.Attributes.Add("onload", "onLoad()");
            }
        }
    }
}