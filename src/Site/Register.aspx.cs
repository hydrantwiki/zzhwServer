using System;
using System.Drawing;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using TreeGecko.Library.Common.Enums;
using TreeGecko.Library.Net.Objects;

namespace HydrantWiki.Web
{

    public partial class Register : System.Web.UI.Page
    {
        HydrantWikiManager m_Manager = new HydrantWikiManager();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void txtUserName_TextChanged(object sender, EventArgs e)
        {
            string userName = txtUserName.Text;


            User user = m_Manager.GetUser(UserSources.HydrantWiki, userName.ToLower());

            if (user == null)
            {
                lblUserNameAvailable.Text = "Available";
                lblUserNameAvailable.ForeColor = Color.Green;
                btnCreateAccount.Enabled = true;
            }
            else
            {
                lblUserNameAvailable.Text = "Unavailable";
                lblUserNameAvailable.ForeColor = Color.DarkRed;
                btnCreateAccount.Enabled = false;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text;
            string email = txtEmailAddress.Text;
            string pwd1 = txtPasswordFirst.Text;
            string pwd2 = txtPasswordReenter.Text;
            
            if (!string.IsNullOrEmpty(userName)
                && !string.IsNullOrEmpty(email)
                && !string.IsNullOrEmpty(pwd1)
                && !string.IsNullOrEmpty(pwd2))
            {
                if (pwd1.Equals(pwd2))
                {
                    User user = m_Manager.GetUser(UserSources.HydrantWiki, userName.ToLower());

                    if (user == null)
                    {
                        user = new User();
                        user.Username = userName.ToLower();
                        user.UserSource =UserSources.HydrantWiki;
                        user.UserType = UserTypes.User;
                        user.Active = true;
                        user.IsVerified = false;
                        user.LastModifiedDateTime = DateTime.UtcNow;
                        user.EmailAddress = email.ToLower();
                        user.DisplayName = userName;
                        m_Manager.Persist(user);

                        string salt = TGUserPassword.GenerateSalt(userName);

                        TGUserPassword userPassword = new TGUserPassword();
                        userPassword.UserGuid = user.Guid;
                        userPassword.Salt = salt;
                        userPassword.HashedPassword = TGUserPassword.HashPassword(salt, pwd1);
                        userPassword.LastModifiedDateTime = DateTime.UtcNow;
                        m_Manager.Persist(userPassword);

                        TGUserEmailValidation validation = new TGUserEmailValidation(user);
                        m_Manager.Persist(validation);

                        //Enqueue the email request
                        m_Manager.SendUserValidationEmail(user, validation);

                        //TODO notify that it was sent
                        Response.Redirect("/default.aspx");
                    }
                }
            }

           
        }

        protected void txtEmailAddress_TextChanged(object sender, EventArgs e)
        {
            string email = txtEmailAddress.Text;

            ServerDataManager sdm = new ServerDataManager();
            UserEmail userEmail = sdm.GetUserEmail(email);

            if (userEmail == null)
            {
                lblEmailAvailable.Text = "";
                btnCreateAccount.Enabled = true;
            }
            else
            {
                lblEmailAvailable.Text = "Already in use.";
                lblEmailAvailable.ForeColor = Color.DarkRed;
                btnCreateAccount.Enabled = false;
            }
        }
    }
}