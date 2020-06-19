using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SoloistWebClient.Startup))]
namespace SoloistWebClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
