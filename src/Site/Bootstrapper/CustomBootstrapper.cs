using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Session;
using Nancy.TinyIoc;
using Site.Helpers;

namespace Site.Bootstrapper
{
    public class CustomBoostrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureRequestContainer(TinyIoCContainer _container, Nancy.NancyContext _context)
        {
            base.ConfigureRequestContainer(_container, _context);
            _container.Register<IUserMapper, UserMapper>();
        }

        protected override void ApplicationStartup(TinyIoCContainer _container, IPipelines _pipelines)
        {
            CookieBasedSessions.Enable(_pipelines);
            Nancy.Security.Csrf.Enable(_pipelines);

            var formsAuthConfiguration = new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "/login",
                UserMapper = _container.Resolve<IUserMapper>(),
            };

            FormsAuthentication.Enable(_pipelines, formsAuthConfiguration);
        }

        protected override void ConfigureConventions(NancyConventions _conventions)
        {
            base.ConfigureConventions(_conventions);
           
            _conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("style", @"Style")
            );

            _conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("js", @"js")
            );

            _conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("images", @"images")
            );
        }
    }
}