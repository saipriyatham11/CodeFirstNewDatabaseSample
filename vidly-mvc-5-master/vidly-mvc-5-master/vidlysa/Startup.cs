using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(vidlysa.Startup))]
namespace vidlysa
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
