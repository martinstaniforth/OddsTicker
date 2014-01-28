using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OddsTicker.Model
{
  public class BookmakersSentiment
  {
    public decimal HomeWinVariancePct { get; set; }
    public decimal DrawVariancePct { get; set; }
    public decimal AwayWinVariancePct { get; set; }
    public decimal PayoutVariancePct { get; set; }

    public BookmakersSentiment VarySentiment(Random random)
    {
      HomeWinVariancePct = Math.Max(-.05m, Math.Min(.05m, HomeWinVariancePct + random.Next(-10, 10) / 1000m));
      DrawVariancePct = Math.Max(-.05m, Math.Min(.05m, DrawVariancePct + random.Next(-10, 10) / 1000m));
      AwayWinVariancePct = Math.Min(-.05m, Math.Min(.05m, AwayWinVariancePct + random.Next(-10, 10) / 1000m));

      PayoutVariancePct = Math.Min(-.05m, Math.Min(.05m, PayoutVariancePct + random.Next(-10, 10) / 1000m));

      return this;
    }
  }
}