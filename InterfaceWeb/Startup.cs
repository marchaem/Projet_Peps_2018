using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InterfaceWeb.Startup))]
namespace InterfaceWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
