using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace Site.Modules
{
    public class RegisterModule : NancyModule
    {
        public RegisterModule()
        {
            Get["/register"] = _parameters =>
            {
                return View["register.sshtml"];
            };

            Post["/register"] = _parameters =>
            {
                Response response = HandleRegister(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string HandleRegister(DynamicDictionary _parameters)
        {
            return "{ \"Result\":\"Success\"}";
        }

    }
}