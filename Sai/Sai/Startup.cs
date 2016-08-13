using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(sai.Startup))]
namespace sai
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
