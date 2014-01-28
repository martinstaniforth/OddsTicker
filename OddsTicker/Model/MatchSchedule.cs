using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

namespace OddsTicker.Model
{
  public class MatchSchedule
  {
    public static IObservable<SentimentDelta> GetMatchProbabilities(Random random)
    {
      var matchStarts = DateTimeOffset.Now.AddMinutes(1);
      var matches = MatchSchedule.GetMatchSchedule()
                                 .OrderByDescending(x => x.TeamA)
                                 .ToObservable()
                                 .Zip(Observable.Interval(TimeSpan.FromSeconds(1)), (x, y) => x);

      return matches.SelectMany(match =>
      {
        return Observable.Create<SentimentDelta>(observer =>
        {
          var probabilities =
            match.GenerateProbabilities((decimal)random.NextDouble(), matchStarts, random)
                 .Publish()
                 .RefCount();

          var movements =
            probabilities.DistinctUntilChanged(x => (new
            {
              Home = x.HomeWinProbability,
              Draw = x.DrawProbability,
              Away = x.AwayWinProbability
            }).GetHashCode());

          var mergedDeltas = movements.FirstProbabilityAsDelta(match)
                                      .Merge(movements.ProbabilityDeltas(2, 1, match));

          return mergedDeltas.Subscribe(observer);
        });
      })
      .Publish()
      .RefCount();
    }

    public static IEnumerable<Match> GetMatchSchedule()
    {
      var teamsWithSkills = new Dictionary<string, TeamMetrics>()
      {
        { "Arsenal", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 1.43967250529824m, HomeDefenceWeakness = 0.784060438807664m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 1.26279529485448m, AwayDefenceWeakness = 0.614777111598167m } },
        { "Aston Villa", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.845182676109914m, HomeDefenceWeakness = 1.26396962401895m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.926545710762727m, AwayDefenceWeakness = 1.329903816545m } },
        { "Birmingham", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 1.40163385077979m, HomeDefenceWeakness = 0.716732233378349m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 1.42132973205723m, AwayDefenceWeakness = 0.749542630463528m } },
        { "Blackburn", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 1.06399550781604m, HomeDefenceWeakness = 0.742958918051397m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 1.00326854950776m, AwayDefenceWeakness = 0.761497636169326m } },
        { "Bolton", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.942634086257178m, HomeDefenceWeakness = 1.1743291644648m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.938680445462197m, AwayDefenceWeakness = 1.08681870052711m } },
        { "Burnley", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 1.25527559910881m, HomeDefenceWeakness = 0.768402716614801m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 1.42289550427652m, AwayDefenceWeakness = 0.846631767710616m } },
        { "Chelsea", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 1.29403879942761m, HomeDefenceWeakness = 0.638835065468851m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 1.1852895699998m, AwayDefenceWeakness = 0.640498487510642m } },
        { "Everton", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 1.58240802796747m, HomeDefenceWeakness = 0.808329908206604m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 1.65658700800501m, AwayDefenceWeakness = 0.809679931892695m } },
        { "Fulham", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.833589943304291m, HomeDefenceWeakness = 1.29097919480164m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.860783277552698m, AwayDefenceWeakness = 1.26867969641531m } },
        { "Hull", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.798087199087072m, HomeDefenceWeakness = 1.01540328420723m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.742567474996575m, AwayDefenceWeakness = 1.1614469179633m } },
        { "Liverpool", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.518774793051606m, HomeDefenceWeakness = 1.14771103673693m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.613782709960269m, AwayDefenceWeakness = 1.11145325773906m } },
        { "Man City", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.797362653286721m, HomeDefenceWeakness = 1.38218542657507m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.821638972070538m, AwayDefenceWeakness = 1.36540656076222m } },
        { "Man United", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.906044523339432m, HomeDefenceWeakness = 1.09447478128119m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.939071888517018m, AwayDefenceWeakness = 1.16072237216295m } },
        { "Portsmouth", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.66513304472259m, HomeDefenceWeakness = 0.874092341416633m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.612216937740982m, AwayDefenceWeakness = 0.821272664698317m } },
        { "Stoke", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.736500806057203m, HomeDefenceWeakness = 0.950423737106844m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.809112794316247m, AwayDefenceWeakness = 1.07667505932219m } },
        { "Sunderland", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.906769069139783m, HomeDefenceWeakness = 1.00483432172705m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.860000391443055m, AwayDefenceWeakness = 0.917637256145054m } },
        { "Tottenham", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 1.14623145615592m, HomeDefenceWeakness = 0.833773706770008m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 1.34499833636702m, AwayDefenceWeakness = 0.894814063433985m } },
        { "West Ham", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 1.02776821779847m, HomeDefenceWeakness = 1.06903098271779m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.9641242440256m, AwayDefenceWeakness = 1.07558824062166m } },
        { "Wigan", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.95676272936403m, HomeDefenceWeakness = 0.977433307889535m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.727692638913354m, AwayDefenceWeakness = 1.01545093919249m } },
        { "Wolves", new TeamMetrics { HomeAverageGoals = 1.55789473684211m, HomeAttackStrength = 0.882134511927835m, HomeDefenceWeakness = 1.46203980975868m, AwayAverageGoals = 1.23947368421053m, AwayAttackStrength = 0.886618519170924m, AwayDefenceWeakness = 1.29150288912638m } },

      }

      .OrderBy(x => Guid.NewGuid())
        .ToList();

      for (int i = 0; i < teamsWithSkills.Count; i += 2)
      {
        yield return
          new Match(teamsWithSkills[i].Key,
                    teamsWithSkills[i + 1].Key,
                    teamsWithSkills[i].Value,
                    teamsWithSkills[i + 1].Value);
      }
    }
  }


}