using Owin;
using Microsoft.Owin;

[assembly: OwinStartupAttribute(typeof(CW.Soloist.WebApplication.Startup))]
namespace CW.Soloist.WebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
