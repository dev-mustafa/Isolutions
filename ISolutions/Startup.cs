using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ISolutions.Portal.Startup))]
namespace ISolutions.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
