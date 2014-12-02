using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Nancy.Authentication.Forms;
using Site.Helpers;
using TreeGecko.Library.Common.Enums;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Common.Security;
using TreeGecko.Library.Net.Objects;

namespace Site.Modules
{
    public class LoginModule : NancyModule
    {
        public LoginModule()
        {
            Get["/login"] = _parameters =>
            {
                return View["login.sshtml"];
            };

            Post["/login"] = _parameters =>
            {
                User user;

                string username = Request.Headers["Username"].First();
                string password = Request.Headers["Password"].First();

                string result = AuthHelper.Authorize(username, password, out user);

                Response response;

                if (user != null)
                {
                    response = this.LoginWithoutRedirect(user.Guid, DateTime.UtcNow.AddDays(30));
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(result);

                    response.Contents = _stream => _stream.Write(jsonBytes, 0, jsonBytes.Length); 
                }
                else
                {
                    response = result;
                }
                 
                response.ContentType = "application/json";
                return response;
            };

            Get["/logout"] = _parameters =>
            {
                return this.LogoutAndRedirect("/logoutcomplete");
            };

            Get["/logoutcomplete"] = _parameters =>
            {
                return View["logout.sshtml"];
            };

            Get["/reset"]  = _parameters =>
            {
                return View["reset.sshtml"];
            };

            Post["/reset"] = _parameters =>
            {
                Response response= HandleResetPassword(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/register"] = _parameters =>
            {
                return View["register.sshtml"];
            };

            Post["/register"] = _parameters =>
            {
                Response response = (Response)Register(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string Register(DynamicDictionary _parameters)
        {
            string username = Request.Headers["Username"].First();
            string email = Request.Headers["Email"].First();
            string password = Request.Headers["Password"].First();

            HydrantWikiManager hwm = new HydrantWikiManager();

            User user = hwm.GetUser(UserSources.HydrantWiki, username);

            if (user == null)
            {
                user = new User
                {
                    UserSource = UserSources.HydrantWiki,
                    IsVerified = false,
                    Active = true,
                    DisplayName = username,
                    EmailAddress = email,
                    UserType = UserTypes.User,
                    Username = username
                };
                hwm.Persist(user);

                TGUserPassword userPassword = TGUserPassword.GetNew(user.Guid, username, password);
                hwm.Persist(userPassword);

                TGUserEmailValidation validation = new TGUserEmailValidation(user);
                hwm.Persist(validation);

                NameValueCollection nvc = new NameValueCollection
                {
                    {"SystemUrl", Config.GetSettingValue("SystemUrl")},
                    {"ValidationText", validation.ValidationText }
                };
                hwm.SendCannedEmail(user, CannedEmailNames.ValidateEmailAddress, nvc);

                return "{ \"Result\":\"Success\" }";
            }

            return "{ \"Result\":\"UsernameNotAvailable\" }";
        }


        private string HandleResetPassword(DynamicDictionary _parameters)
        {
            string email = Request.Headers["EmailAddress"].First();

            if (email != null)
            {
                HydrantWikiManager hwm = new HydrantWikiManager();
                User user = hwm.GetUserByEmail(UserSources.HydrantWiki, email.ToLower());

                if (user != null)
                {
                    string newPassword = RandomString.GetRandomString(10);

                    TGUserPassword up = TGUserPassword.GetNew(user.Guid, user.Username, newPassword);
                    hwm.Persist(up);

                    NameValueCollection nvc = new NameValueCollection
                    {
                        {"Password", newPassword}
                    };

                    hwm.SendCannedEmail(user, CannedEmailNames.ResetPasswordEmail, nvc);
                }

                return "{ \"Result\":\"Success\"}";

            }

            return "{ \"Result\":\"Failure\"}";
        }

        
        
    }
}