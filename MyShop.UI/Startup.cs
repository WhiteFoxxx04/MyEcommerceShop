using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyShop.UI.Startup))]
namespace MyShop.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
