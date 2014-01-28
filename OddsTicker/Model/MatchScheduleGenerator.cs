using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reactive.Linq;

namespace OddsTicker.Model
{
  public class MatchScheduleGenerator
  {
    public IObservable<SentimentDelta> SentimentDeltas { get; set; }
    public IObservable<BookmakerOdd> CombinedBookmakerFeeds { get; set; }

    public MatchScheduleGenerator()
    {
      GenerateMatchSchedules();
    }

    public void GenerateMatchSchedules()
    {
      var random = new Random();

      SentimentDeltas =
        Observable.Defer(() => Observable.Start(() => MatchSchedule.GetMatchProbabilities(random)))
                  .Switch()
                  .Delay(TimeSpan.FromSeconds(3))
                  .Repeat()
                  .Publish()
                  .RefCount();

      var bills = new Bookmaker("Billy Hills");
      var brokeLads = new Bookmaker("Brokelads", 0.95m);
      var panicAll = new Bookmaker("Panic-all", 0.97m);


      var billsFeed = bills.GetBookmakerOdds(SentimentDeltas, random);
      var brokeLadsFeed = brokeLads.GetBookmakerOdds(SentimentDeltas, random);
      var panicAllFeed = panicAll.GetBookmakerOdds(SentimentDeltas, random);

      CombinedBookmakerFeeds = billsFeed.Merge(brokeLadsFeed)
                                        .Merge(panicAllFeed)
                                        .Publish()
                                        .RefCount();
    }

  }
}