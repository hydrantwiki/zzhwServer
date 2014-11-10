using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using TreeGecko.Library.Net.Objects;

namespace Site.RestModules
{
    public class EmailValidationModule: NancyModule
    {
        public EmailValidationModule()
        {
            Get["/rest/emailvalidation/{validationtoken}"] = _parameters =>
            {
                Response response = (Response) HandleGet(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string HandleGet(DynamicDictionary _parameters)
        {
            string validationToken = _parameters["validationtoken"];

            if (!string.IsNullOrEmpty(validationToken))
            {
                HydrantWikiManager hwManager = new HydrantWikiManager();

                TGUserEmailValidation uev = hwManager.GetTGUserEmailValidation(validationToken);

                if (uev != null
                    && uev.ParentGuid != null)
                {
                    User user = (User)hwManager.GetUser(uev.ParentGuid.Value);

                    if (user != null)
                    {
                        user.IsVerified = true;

                        hwManager.Persist(user);
                        hwManager.Delete(uev);

                        return @"{ ""Result"":""Success"" }";
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

            return @"{ ""Result"":""Failure"" }";
        }

        
    }
}