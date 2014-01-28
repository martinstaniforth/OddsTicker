using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace OddsTicker.Model
{
  public class Match
  {
    public Match(string teamA, string teamB,
      TeamMetrics teamAMetrics, TeamMetrics teamBMetrics)
    {
      TeamA = teamA;
      TeamB = teamB;
      TeamAMetrics = teamAMetrics;
      TeamBMetrics = teamBMetrics;
    }

    public string TeamA { get; private set; }
    public string TeamB { get; private set; }
    public TeamMetrics TeamAMetrics { get; set; }
    public TeamMetrics TeamBMetrics { get; set; }


    public IObservable<SentimentProbabilities> GenerateProbabilities(decimal startingHomeOdds, DateTimeOffset matchStarts, Random random)
    {
      var matchProbabilities = new SentimentProbabilities(TeamAMetrics, TeamBMetrics);
      var probabilityStream = Observable.Generate(matchProbabilities,
                                             p => true,
                                             p => p.UpdateProbabilties(random),
                                             p => p,
                                             p => TimeSpan.FromSeconds(0.5 * random.NextDouble() + 0.5)
                                          );
      return probabilityStream.TakeUntil(matchStarts);

    }
  }
}