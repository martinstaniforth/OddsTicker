using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OddsTicker.Model
{
  public class SentimentProbabilities
  {
    private Dictionary<string, decimal> scoreLineProbabilties;
    private const double chanceOfNoChange = 0.75;

    public SentimentProbabilities(TeamMetrics homeTeamMetrics, TeamMetrics awayTeamMetrics)
    {
      HomeTeamMetrics = homeTeamMetrics;
      AwayTeamMetrics = awayTeamMetrics;
      this.scoreLineProbabilties = new Dictionary<string, decimal>();
      CalculateProbabilities();
    }

    public TeamMetrics HomeTeamMetrics { get; private set; }
    public TeamMetrics AwayTeamMetrics { get; private set; }

    public decimal HomeWinProbability { get; private set; }
    public decimal DrawProbability { get; private set; }
    public decimal AwayWinProbability { get; private set; }

    public decimal this[int homeGoals, int awayGoals]
    {
      get
      {
        var scoreLine = string.Format("{0}-{1}", homeGoals, awayGoals);

        if (homeGoals > 10 || awayGoals > 10 || homeGoals < 0 || awayGoals < 0 ||
          !this.scoreLineProbabilties.ContainsKey(scoreLine))
          throw new IndexOutOfRangeException();

        return this.scoreLineProbabilties[scoreLine];
      }
    }

    public SentimentProbabilities UpdateProbabilties(Random random)
    {
      if (random.NextDouble() >= chanceOfNoChange)
      {
        var newHomeTeamMetrics = HomeTeamMetrics.UpdateMetrics(random);
        var newAwayTeamMetrics = AwayTeamMetrics.UpdateMetrics(random);
        return new SentimentProbabilities(newHomeTeamMetrics, newAwayTeamMetrics);
      }
      return this;
    }

    private void CalculateProbabilities()
    {
      Func<int, int> factorial = null;
      factorial = num => num <= 1 ? 1 : num * factorial(num - 1);
      Func<decimal, int, decimal> poisson =
        (lambda, x) => ((decimal)(Math.Exp((double)-lambda) * Math.Pow((double)lambda, (double)x)) / ((decimal)factorial(x)));


      var scoreLines =
        from homeGoal in Enumerable.Range(0, 10)
        from awayGoal in Enumerable.Range(0, 10)
        select new { HomeGoal = homeGoal, AwayGoal = awayGoal };

      scoreLines.ToList().ForEach(scoreline =>
      {
        var scoreLineProb = poisson(HomeTeamMetrics.HomeAverageGoals * HomeTeamMetrics.HomeAttackStrength * AwayTeamMetrics.AwayDefenceWeakness, scoreline.HomeGoal) *
                            poisson(AwayTeamMetrics.AwayAverageGoals * AwayTeamMetrics.AwayAttackStrength * HomeTeamMetrics.HomeDefenceWeakness, scoreline.AwayGoal);

        this.scoreLineProbabilties[string.Format("{0}-{1}", scoreline.HomeGoal, scoreline.AwayGoal)] = scoreLineProb;

        HomeWinProbability += scoreline.HomeGoal > scoreline.AwayGoal ? scoreLineProb : 0.0m;
        DrawProbability += scoreline.HomeGoal == scoreline.AwayGoal ? scoreLineProb : 0.0m;
        AwayWinProbability += scoreline.HomeGoal < scoreline.AwayGoal ? scoreLineProb : 0.0m;
      });
    }
  }

}