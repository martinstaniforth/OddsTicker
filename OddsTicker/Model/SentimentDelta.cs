using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OddsTicker.Model
{
  public class SentimentDelta
  {
    public string TeamA { get; set; }
    public string TeamB { get; set; }

    public decimal HomeWinProbabilityDelta { get; set; }
    public decimal DrawProbabilityDelta { get; set; }
    public decimal AwayWinProbabilityDelta { get; set; }

    public decimal HomeWinProbabilityCurrent { get; set; }
    public decimal DrawProbabilityCurrent { get; set; }
    public decimal AwayWinProbabilityCurrent { get; set; }

    public bool OpeningPrediction { get; set; }
    public bool ClosingPrediction { get; set; }
  }
}