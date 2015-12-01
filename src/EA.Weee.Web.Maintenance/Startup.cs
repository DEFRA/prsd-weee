using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EA.Weee.Web.Maintenance.Startup))]
namespace EA.Weee.Web.Maintenance
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
         //   ConfigureAuth(app);
        }
    }
}
