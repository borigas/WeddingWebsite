using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeddingWebsite.Startup))]
namespace WeddingWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
