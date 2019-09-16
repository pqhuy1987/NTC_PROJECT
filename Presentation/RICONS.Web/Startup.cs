using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(RICONS.Web.Startup))]
namespace RICONS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
