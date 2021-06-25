using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Class_Relax.Startup))]
namespace Class_Relax
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
