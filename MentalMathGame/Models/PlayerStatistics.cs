using MentalMathGame.Models.Enums;

namespace MentalMathGame.Models;

public class PlayerStatistics
{
    public int TotalGamesPlayed { get; set; }
    public int TotalPoints { get; set; }
    public double GlobalAccuracy { get; set; }
    public Dictionary<string, int> BestScorePerMode { get; set; } = [];
    public Dictionary<string, double> OperationAccuracy { get; set; } = [];
    public Dictionary<string, int> GamesPerDay { get; set; } = [];
}
