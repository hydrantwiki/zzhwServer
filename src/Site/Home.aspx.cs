using System;

namespace HydrantWiki.Web
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User user = (User)Session["User"];

            if (user != null)
            {
                lblUserWelcome.Text = string.Format("Welcome {0}", user.DisplayName);

                //Disable superuser and admin
                if (user.UserType == UserTypes.User)
                {
                    mnuMenu.Items[0].Expanded = true;

                    mnuMenu.Items[1].Enabled = false;
                    mnuMenu.Items[1].Visible = false;

                    mnuMenu.Items[2].Enabled = false;
                    mnuMenu.Items[2].Visible = false;
                }

                //Disable admin
                if (user.UserType == UserTypes.SuperUser)
                {
                    mnuMenu.Items[0].Expanded = true;
                    mnuMenu.Items[1].Expanded = true;

                    mnuMenu.Items[2].Enabled = false;
                    mnuMenu.Items[2].Visible = false;
                }

                if (user.UserType == UserTypes.Administrator)
                {
                    mnuMenu.Items[0].Expanded = true;
                    mnuMenu.Items[1].Expanded = true;
                    mnuMenu.Items[2].Expanded = true;
                }
            }
            else
            {
                Response.Redirect("/default.aspx");
            }
        }
    }
}