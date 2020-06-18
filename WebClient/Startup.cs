using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebClient.Startup))]
namespace WebClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
