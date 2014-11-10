using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;

namespace Site.Modules
{
    public class ReviewTagsModule : NancyModule
    {
        public ReviewTagsModule()
        {
            this.RequiresAuthentication();
            //this.RequiresClaims(new [] { "SuperUser" });

            Get["/reviewtags"] = _parameters =>
            {
                return View["reviewtags.sshtml"];
            };

           

        }
    }
}