using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace Site.Modules
{
    public class RootModule: NancyModule
    {
        public RootModule()
        {
            Get["/"] = _parameters =>
            {
                return View["default.sshtml"];
            };
        }
    }
}