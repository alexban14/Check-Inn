using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Check_Inn.Startup))]
namespace Check_Inn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
