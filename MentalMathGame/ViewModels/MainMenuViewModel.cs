using MentalMathGame.Models;
using MentalMathGame.Models.Enums;

namespace MentalMathGame.ViewModels;

public class MainMenuViewModel : BaseViewModel
{
    private readonly Action<GameMode> _onModeSelected;
    private readonly Action _onLogout;

    public PlayerProfile CurrentPlayer { get; }

    public string WelcomeMessage => $"Bonjour, {CurrentPlayer.Username} !";
    public int TotalGames => CurrentPlayer.Statistics.TotalGamesPlayed;
    public int TotalPoints => CurrentPlayer.Statistics.TotalPoints;
    public double GlobalAccuracy => CurrentPlayer.Statistics.GlobalAccuracy;
    public int BadgesUnlocked => CurrentPlayer.Badges.Count(b => b.IsUnlocked);
    public int TotalBadges => 7;

    public RelayCommand<string> SelectModeCommand { get; }
    public RelayCommand LogoutCommand { get; }

    public MainMenuViewModel(PlayerProfile player, Action<GameMode> onModeSelected, Action onLogout)
    {
        CurrentPlayer = player;
        _onModeSelected = onModeSelected;
        _onLogout = onLogout;

        SelectModeCommand = new RelayCommand<string>(mode =>
        {
            if (Enum.TryParse<GameMode>(mode, out var gameMode))
                _onModeSelected(gameMode);
        });

        LogoutCommand = new RelayCommand(_onLogout);
    }
}
