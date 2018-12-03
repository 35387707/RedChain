using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RelaxBarWeb_MVC.Startup))]
namespace RelaxBarWeb_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
