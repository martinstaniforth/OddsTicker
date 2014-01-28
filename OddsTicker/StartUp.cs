using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OddsTicker.StartUp))]

namespace OddsTicker
{
  public class StartUp
  {
    public void Configuration(IAppBuilder app)
    {
      app.MapSignalR();
    }
  }
}