using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;

namespace Site.Modules
{
    public class TagAppModule : NancyModule
    {
        public TagAppModule()
        {
            Get["/tagapp"] = _parameters =>
            {
                return View["tagapp.sshtml"];
            };
        }
    }
}