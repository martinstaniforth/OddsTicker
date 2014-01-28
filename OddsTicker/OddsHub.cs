using Microsoft.AspNet.SignalR;
using OddsTicker.Model;

namespace OddsTicker
{
  public class OddsHub : Hub
  {
    public OddsHub()
    {
      var subscription = SubscriptionSingleton.Instance;
      subscription.MaybeStartSubscription();
    }


  }
}