using System;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using TreeGecko.Library.Net.Helpers;
using TreeGecko.Library.Net.Objects;

namespace HydrantWiki.Web
{
    public partial class _default : System.Web.UI.Page
    {
        private HydrantWikiManager m_Manager = new HydrantWikiManager();

        protected void Page_Load(object _sender, EventArgs _e)
        {
            if (!IsPostBack)
            {
                if (LoginHelper.LoginAndCreateSession(m_Manager, Context))
                {
                    //Already have a session
                    pnlLogin.Visible = false;
                    pnlEnterApp.Visible = true;
                }
                else
                {
                    pnlLogin.Visible = true;
                    pnlEnterApp.Visible = false;
                }

                lblUserCount.Text = Convert.ToString(m_Manager.GetUserCount());
                lblTagCount.Text = Convert.ToString(m_Manager.GetTagCount());
                lblHydrantCount.Text = Convert.ToString(m_Manager.GetHydrantCount());

                //pnlLogin.DefaultButton = btnLogin.ID;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_userAuthorization"></param>
        private void RememberLogin(TGUserAuthorization _userAuthorization)
        {
            LoginHelper.RememberLogin(Response, _userAuthorization, "hydrantWiki.com");
        }

        protected void btnLogin_Click(object _sender, EventArgs _e)
        {
            string username = txtUserName.Text.ToLower();
            string password = txtPassword.Text;

            User user = m_Manager.GetUser("HWK", username);
            
            if (user != null)
            {
                Session["User"] = user;

                if (LoginHelper.LoginAndCreateSession(m_Manager, Context))
                {
                    if (chkRemember.Checked)
                    {
                        TGUserAuthorization authorization = TGUserAuthorization.GetNew(user.Guid,
                            BrowserHelper.GetBrowserType(Request));
                        m_Manager.Persist(authorization);

                        RememberLogin(authorization);
                    }

                    Response.Redirect("/home.aspx");
                    return;
                }
            }
            
            lblStatus.Text = "Login failed"; 
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            LoginHelper.Logout(Context, "hydrantWiki.com");

            Response.Redirect("/default.aspx");
        }

        protected void btnEnterApp_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Home.aspx");
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Register.aspx");
        }
    }
}