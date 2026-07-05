using System.Collections.ObjectModel;
using MentalMathGame.Models;
using MentalMathGame.Models.Enums;
using MentalMathGame.Services;

namespace MentalMathGame.ViewModels;

public class LeaderboardViewModel : BaseViewModel
{
    private readonly SaveService _saveService;
    private readonly Guid        _currentPlayerId;
    private readonly Action      _onBack;

    private GameMode   _selectedMode       = GameMode.Chrono;
    private Difficulty _selectedDifficulty = Difficulty.Facile;

    public ObservableCollection<LeaderboardEntry> Entries { get; } = [];

    public bool IsChronoSelected    => _selectedMode == GameMode.Chrono;
    public bool IsSerieSelected     => _selectedMode == GameMode.Serie;
    public bool IsSurvieSelected    => _selectedMode == GameMode.Survie;
    public bool IsDéfiSelected      => _selectedMode == GameMode.DéfiDuJour;

    public bool IsFacileSelected    => _selectedDifficulty == Difficulty.Facile;
    public bool IsMoyenSelected     => _selectedDifficulty == Difficulty.Moyen;
    public bool IsDifficileSelected => _selectedDifficulty == Difficulty.Difficile;

    private string _playerRankText = string.Empty;
    public string PlayerRankText
    {
        get => _playerRankText;
        private set => Set(ref _playerRankText, value);
    }

    public RelayCommand<string> SelectModeCommand       { get; }
    public RelayCommand<string> SelectDifficultyCommand { get; }
    public RelayCommand         BackCommand             { get; }

    public LeaderboardViewModel(SaveService saveService, Guid currentPlayerId, Action onBack)
    {
        _saveService     = saveService;
        _currentPlayerId = currentPlayerId;
        _onBack          = onBack;

        SelectModeCommand = new RelayCommand<string>(mode =>
        {
            if (Enum.TryParse<GameMode>(mode, out var gm)) _selectedMode = gm;
            RefreshModeButtons();
            LoadLeaderboard();
        });

        SelectDifficultyCommand = new RelayCommand<string>(diff =>
        {
            if (Enum.TryParse<Difficulty>(diff, out var d)) _selectedDifficulty = d;
            RefreshDifficultyButtons();
            LoadLeaderboard();
        });

        BackCommand = new RelayCommand(_onBack);

        LoadLeaderboard();
    }

    private void LoadLeaderboard()
    {
        Entries.Clear();

        int rank = _saveService.GetPlayerRank(_currentPlayerId, _selectedMode, _selectedDifficulty);
        PlayerRankText = rank > 0
            ? $"Ton classement : #{rank}"
            : "Tu n'es pas encore classé dans ce mode";

        var scores = _saveService.GetLeaderboard(_selectedMode, _selectedDifficulty, 10);
        int pos = 1;
        foreach (var score in scores)
        {
            Entries.Add(new LeaderboardEntry
            {
                Rank            = pos++,
                PlayerId        = score.PlayerId,
                PlayerName      = score.PlayerName,
                Points          = score.Points,
                MaxStreak       = score.MaxStreak,
                Date            = score.Date,
                IsCurrentPlayer = score.PlayerId == _currentPlayerId
            });
        }
    }

    private void RefreshModeButtons()
    {
        OnPropertyChanged(nameof(IsChronoSelected));
        OnPropertyChanged(nameof(IsSerieSelected));
        OnPropertyChanged(nameof(IsSurvieSelected));
        OnPropertyChanged(nameof(IsDéfiSelected));
    }

    private void RefreshDifficultyButtons()
    {
        OnPropertyChanged(nameof(IsFacileSelected));
        OnPropertyChanged(nameof(IsMoyenSelected));
        OnPropertyChanged(nameof(IsDifficileSelected));
    }
}
