using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GHM_ERP.Startup))]
namespace GHM_ERP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
