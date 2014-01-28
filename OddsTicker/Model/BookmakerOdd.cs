using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OddsTicker.Model
{
  public class BookmakerOdd
  {
    public string Bookmaker { get; set; }

    public string TeamA { get; set; }
    public string TeamB { get; set; }

    public decimal HomeWinOdds { get; set; }
    public decimal DrawOdds { get; set; }
    public decimal AwayWinOdds { get; set; }

    public bool OpeningOdds { get; set; }
    public bool ClosingOdds { get; set; }

    public string Overround
    {
      get
      {
        return (1m / HomeWinOdds + 1m / DrawOdds + 1m / AwayWinOdds).TruncatePercentage().ToString();
      }
    }
  }
}