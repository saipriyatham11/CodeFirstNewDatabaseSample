using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(new.Startup))]
namespace new
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
