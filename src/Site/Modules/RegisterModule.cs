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
        }


    }
}