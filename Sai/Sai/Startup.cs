using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sai.Startup))]
namespace Sai
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
