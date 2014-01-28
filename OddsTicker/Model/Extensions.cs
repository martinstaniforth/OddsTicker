using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace OddsTicker.Model
{
  public static class Extensions
  {
    public static decimal TruncatePercentage(this decimal raw)
    {
      return Math.Truncate(raw * 10000) / 100m;
    }

    public static IObservable<T> OverlappingWindow<T>(this IObservable<T> source, int count, int skip)
    {
      var shared = source.Publish().RefCount();
      var windowOpen = shared.Take(count);
      var windowClose = shared.Skip(skip).Take(count);
      var window = shared.Window(windowOpen, _ => windowClose);
      return window.Switch();
    }

    public static IObservable<SentimentProbabilities> GetMatchProbabilities(this IObservable<Match> source, Random random)
    {
      return source.SelectMany(match =>
      {
        return Observable.Create<SentimentProbabilities>(observer =>
        {
          var probabilities =
            match.GenerateProbabilities((decimal)random.NextDouble(), DateTimeOffset.Now.AddSeconds(3), random)
                 .Publish()
                 .RefCount();

          return probabilities.Subscribe(observer);
        });
      });
    }

    public static IObservable<SentimentDelta> FirstProbabilityAsDelta(this IObservable<SentimentProbabilities> source, Match match)
    {
      return Observable.Create<SentimentDelta>(observer =>
      {
        return source.Take(1).Subscribe(first =>
        {
          observer.OnNext(new SentimentDelta
          {
            TeamA = match.TeamA,
            TeamB = match.TeamB,
            HomeWinProbabilityDelta = 0m,
            DrawProbabilityDelta = 0m,
            AwayWinProbabilityDelta = 0m,
            HomeWinProbabilityCurrent = first.HomeWinProbability,
            DrawProbabilityCurrent = first.DrawProbability,
            AwayWinProbabilityCurrent = first.AwayWinProbability,
            OpeningPrediction = true
          });
        },
          observer.OnCompleted);
      });
    }

    public static IObservable<SentimentDelta> ProbabilityDeltas(this IObservable<SentimentProbabilities> source, int count, int skip, Match match)
    {
      return Observable.Create<SentimentDelta>(observer =>
      {
        return source.Buffer(count, skip).Subscribe(buffer =>
        {
          var first = buffer.FirstOrDefault();
          var last = buffer.LastOrDefault();
          var lastInSequence = buffer.Count == 1;

          observer.OnNext(new SentimentDelta
          {
            TeamA = match.TeamA,
            TeamB = match.TeamB,
            HomeWinProbabilityDelta = (first == null || last == null) ? 0m : last.HomeWinProbability - first.HomeWinProbability,
            DrawProbabilityDelta = (first == null || last == null) ? 0m : last.DrawProbability - first.DrawProbability,
            AwayWinProbabilityDelta = (first == null || last == null) ? 0m : last.AwayWinProbability - first.AwayWinProbability,
            HomeWinProbabilityCurrent = last == null ? 0m : last.HomeWinProbability,
            DrawProbabilityCurrent = last == null ? 0m : last.DrawProbability,
            AwayWinProbabilityCurrent = last == null ? 0m : last.AwayWinProbability,
            ClosingPrediction = lastInSequence
          });
        },
          observer.OnCompleted);
      });
    }
  }
}