using MentalMathGame.Models;
using MentalMathGame.Models.Enums;

namespace MentalMathGame.ViewModels;

public class GameResultViewModel : BaseViewModel
{
    private readonly Action _onPlayAgain;
    private readonly Action _onBackToMenu;

    public GameSession Session { get; }

    public string ModeLabel => Session.Mode switch
    {
        GameMode.Chrono     => "Mode Chrono",
        GameMode.Serie      => "Mode Série",
        GameMode.Survie     => "Mode Survie",
        GameMode.DéfiDuJour => "Défi du jour",
        _                   => string.Empty
    };

    public string DifficultyLabel => Session.Difficulty switch
    {
        Difficulty.Facile    => "Facile",
        Difficulty.Moyen     => "Moyen",
        Difficulty.Difficile => "Difficile",
        _                    => string.Empty
    };

    public string AccuracyText => $"{Session.Accuracy:F0} %";

    public string DurationText => Session.Duration.TotalMinutes >= 1
        ? $"{(int)Session.Duration.TotalMinutes}m {Session.Duration.Seconds:D2}s"
        : $"{Session.Duration.Seconds}s";

    public string ResultMessage => Session.Accuracy switch
    {
        >= 90 => "Excellent !",
        >= 70 => "Très bien !",
        >= 50 => "Pas mal !",
        _     => "Continue de t'entraîner !"
    };

    public List<Badge> NewBadges    { get; }
    public bool        HasNewBadges => NewBadges.Count > 0;

    public RelayCommand PlayAgainCommand  { get; }
    public RelayCommand BackToMenuCommand { get; }

    public GameResultViewModel(GameSession session, List<Badge> newBadges,
                               Action onPlayAgain, Action onBackToMenu)
    {
        Session       = session;
        NewBadges     = newBadges;
        _onPlayAgain  = onPlayAgain;
        _onBackToMenu = onBackToMenu;

        PlayAgainCommand  = new RelayCommand(_onPlayAgain);
        BackToMenuCommand = new RelayCommand(_onBackToMenu);
    }
}
