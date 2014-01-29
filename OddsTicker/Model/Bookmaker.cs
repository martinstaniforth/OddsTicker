using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reactive.Linq;

namespace OddsTicker.Model
{
  public class Bookmaker
  {
    private string name;
    private decimal payout;
    private const double probabilityOfBookmakerMissingSentiment = 0.4;

    public Bookmaker(string name, decimal payout = .90m)
    {
      this.name = name;
      this.payout = payout;
    }

    public IObservable<BookmakerOdd> GetBookmakerOdds(IObservable<SentimentDelta> probabilityDeltas, Random random)
    {
      var bookmakerSentiments = new Dictionary<string, BookmakersSentiment>();

      var changeInSentiment = probabilityDeltas.SelectMany(delta =>
      {
        var newMarket = delta.OpeningPrediction;

        var key = string.Format("{0} vs. {1}", delta.TeamA, delta.TeamB);

        if (!bookmakerSentiments.ContainsKey(key))
        {
          bookmakerSentiments.Add(key, new BookmakersSentiment
          {
            PayoutVariancePct = this.payout - .93m
          });
          newMarket = true;
        }

        if (newMarket || delta.ClosingPrediction || random.NextDouble() >= probabilityOfBookmakerMissingSentiment)
        {
          var bookmakerSentiment = bookmakerSentiments[key];
          bookmakerSentiment = bookmakerSentiment.VarySentiment(random);
          var bookmakerSentimentNorm = 1 + (bookmakerSentiment.HomeWinVariancePct
                                         + bookmakerSentiment.DrawVariancePct
                                         + bookmakerSentiment.AwayWinVariancePct);

          var newHomeWinImpliedProbability = ((delta.HomeWinProbabilityCurrent + bookmakerSentiment.HomeWinVariancePct) / bookmakerSentimentNorm);
          var newDrawImpliedProbability = ((delta.DrawProbabilityCurrent + bookmakerSentiment.DrawVariancePct) / bookmakerSentimentNorm);
          var newAwayWinImpliedProbability = ((delta.AwayWinProbabilityCurrent + bookmakerSentiment.AwayWinVariancePct) / bookmakerSentimentNorm);

          var bookmakerOdd = new BookmakerOdd
          {
            Bookmaker = this.name,
            TeamA = delta.TeamA,
            TeamB = delta.TeamB,
            HomeWinOdds = Math.Round((this.payout + bookmakerSentiment.PayoutVariancePct) / newHomeWinImpliedProbability, 2),
            DrawOdds = Math.Round((this.payout + bookmakerSentiment.PayoutVariancePct) / newDrawImpliedProbability, 2),
            AwayWinOdds = Math.Round((this.payout + bookmakerSentiment.PayoutVariancePct) / newAwayWinImpliedProbability, 2),
            OpeningOdds = newMarket,
            ClosingOdds = delta.ClosingPrediction
          };

          if (delta.ClosingPrediction)
            bookmakerSentiments.Remove(key);

          return Observable.Return(bookmakerOdd);
        }
        else
        {
          return Observable.Empty<BookmakerOdd>();
        }
      });
      return changeInSentiment.Publish().RefCount();
    }

  }


}