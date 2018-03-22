using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebEurostral.Startup))]
namespace WebEurostral
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
