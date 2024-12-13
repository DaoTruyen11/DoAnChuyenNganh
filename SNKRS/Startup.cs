using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SNKRS.Startup))]
namespace SNKRS
{
    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();    // Map SignalR cho Owin

        }


        public void InitAdmin()
        {

        }
    }
}
