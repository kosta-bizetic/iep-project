using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(iep_project.Startup))]
namespace iep_project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
