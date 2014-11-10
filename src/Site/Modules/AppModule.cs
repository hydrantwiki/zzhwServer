using Nancy;
using Nancy.Security;

namespace Site.Modules
{
    public class AppModule : NancyModule
    {
        public AppModule()
        {
            this.RequiresAuthentication();

            Get["/home"] = _parameters =>
            {
                return View["home.sshtml"];
            };

            Get["/mytags"] = _parameters =>
            {
                return View["mytags.sshtml"];
            };

            Get["/hydrants"] = _parameters =>
            {
                return View["hydrants.sshtml"];
            };
        }

        
    }
}