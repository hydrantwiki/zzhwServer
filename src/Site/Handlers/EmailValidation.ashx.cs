using System.Web;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using TreeGecko.Library.Net.Objects;

namespace HydrantWiki.Web.Handlers
{
    /// <summary>
    /// Summary description for EmailValidation
    /// </summary>
    public class EmailValidation : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string validationText = context.Request.QueryString["validation"];

            if (!string.IsNullOrEmpty(validationText))
            {
                HydrantWikiManager hwManager = new HydrantWikiManager();

                TGUserEmailValidation uev = hwManager.GetTGUserEmailValidation(validationText);

                if (uev != null
                    && uev.ParentGuid!=null)
                {
                    User user = (User)hwManager.GetUser(uev.ParentGuid.Value);

                    if (user != null)
                    {
                        user.IsVerified = true;

                        hwManager.Persist(user);
                        hwManager.Delete(uev);
                    }
                    else
                    {
                        //User not found.
                    }
                }
                else
                {
                    //Validation text not found in database
                }
            }
            else
            {
                //Validation text not supplied.
            }

            context.Response.Redirect("/default.aspx");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}