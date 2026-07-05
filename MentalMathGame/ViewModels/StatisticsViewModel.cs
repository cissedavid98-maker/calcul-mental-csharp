using MentalMathGame.Models;
using MentalMathGame.Services;

namespace MentalMathGame.ViewModels;

public class StatisticsViewModel : BaseViewModel
{
    private readonly PlayerProfile _player;
    private readonly Action        _onBack;

    public string TotalGamesText  => _player.Statistics.TotalGamesPlayed.ToString();
    public string TotalPointsText => _player.Statistics.TotalPoints.ToString("N0");
    public string AccuracyText    => $"{_player.Statistics.GlobalAccuracy:F0}%";

    public int BestChrono  => GetBestScore("Chrono");
    public int BestSerie   => GetBestScore("Serie");
    public int BestSurvie  => GetBestScore("Survie");
    public int BestDéfi    => GetBestScore("DéfiDuJour");

    public string AdditionText       => GetAccuracyText("Addition");
    public string SoustractionText   => GetAccuracyText("Soustraction");
    public string MultiplicationText => GetAccuracyText("Multiplication");
    public string DivisionText       => GetAccuracyText("Division");

    public double AdditionValue       => GetAccuracyValue("Addition");
    public double SoustractionValue   => GetAccuracyValue("Soustraction");
    public double MultiplicationValue => GetAccuracyValue("Multiplication");
    public double DivisionValue       => GetAccuracyValue("Division");

    public string StrongestOperation { get; }
    public string WeakestOperation   { get; }

    public RelayCommand BackCommand { get; }

    public StatisticsViewModel(PlayerProfile player, StatisticsService statsService, Action onBack)
    {
        _player = player;
        _onBack = onBack;

        statsService.UpdateOperationAccuracy(player);
        StrongestOperation = statsService.GetStrongestOperation(player);
        WeakestOperation   = statsService.GetWeakestOperation(player);

        BackCommand = new RelayCommand(_onBack);
    }

    private int GetBestScore(string mode) =>
        _player.Statistics.BestScorePerMode.TryGetValue(mode, out int v) ? v : 0;

    private string GetAccuracyText(string op) =>
        _player.Statistics.OperationAccuracy.TryGetValue(op, out double v) ? $"{v:F0}%" : "—";

    private double GetAccuracyValue(string op) =>
        _player.Statistics.OperationAccuracy.TryGetValue(op, out double v) ? v : 0;
}
