using System;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OddsTicker.Model
{
  public sealed class SubscriptionSingleton
  {
    private bool isStarted = false;
    private IDisposable sentimentSubscription;
    private IDisposable bookmakerOddSubscription;

    private static readonly Lazy<SubscriptionSingleton> lazy = new Lazy<SubscriptionSingleton>(() => new SubscriptionSingleton());
    public static SubscriptionSingleton Instance { get { return lazy.Value; } }
    private SubscriptionSingleton() { }

    public void MaybeStartSubscription()
    {
      if (!isStarted)
      {
        var matchScheduler = new MatchScheduleGenerator();
        sentimentSubscription = matchScheduler.SentimentDeltas.Subscribe(BroadcastPunterSentimentUpdate);
        bookmakerOddSubscription = matchScheduler.CombinedBookmakerFeeds.Subscribe(BroadcastBookmakerPriceUpdate);

        isStarted = true;
      }
    }

    private void BroadcastPunterSentimentUpdate(SentimentDelta sentiment)
    {
      var context = GlobalHost.ConnectionManager.GetHubContext<OddsHub>();
      context.Clients.All.broadcastPunterSentimentUpdate(sentiment);
    }

    private void BroadcastBookmakerPriceUpdate(BookmakerOdd odd)
    {
      var context = GlobalHost.ConnectionManager.GetHubContext<OddsHub>();
      context.Clients.All.broadcastBookmakerPriceUpdate(odd);
    }

  }
}