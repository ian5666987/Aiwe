using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Aiwe.Startup))]
namespace Aiwe
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
