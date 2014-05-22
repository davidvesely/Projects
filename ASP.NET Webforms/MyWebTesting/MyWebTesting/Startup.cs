using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyWebTesting.Startup))]
namespace MyWebTesting
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
