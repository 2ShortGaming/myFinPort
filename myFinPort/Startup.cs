using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(myFinPort.Startup))]
namespace myFinPort
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
