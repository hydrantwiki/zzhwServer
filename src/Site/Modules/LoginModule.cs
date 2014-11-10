using System;
using System.Linq;
using System.Text;
using HydrantWiki.Library.Objects;
using Nancy;
using Nancy.Authentication.Forms;
using Site.Helpers;

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
                return this.LogoutAndRedirect("/");
            };
        }

        
    }
}