using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsTicker.Model
{
  public class TeamMetrics
  {
    public decimal HomeAverageGoals { get; set; }
    public decimal HomeAttackStrength { get; set; }
    public decimal HomeDefenceWeakness { get; set; }

    public decimal AwayAverageGoals { get; set; }
    public decimal AwayAttackStrength { get; set; }
    public decimal AwayDefenceWeakness { get; set; }

    public decimal HomeExpectedGoals { get { return HomeAverageGoals * HomeAttackStrength * AwayDefenceWeakness; } }
    public decimal AwayExpectedGoals { get { return AwayAverageGoals * AwayAttackStrength * HomeDefenceWeakness; } }

    public TeamMetrics UpdateMetrics(Random random)
    {
      HomeAttackStrength += random.Next(-10, 10) / 400.0m;
      HomeDefenceWeakness += random.Next(-10, 10) / 400.0m;
      AwayAttackStrength += random.Next(-10, 10) / 400.0m;
      AwayDefenceWeakness += random.Next(-10, 10) / 400.0m;

      return this;
    }

  }
}
