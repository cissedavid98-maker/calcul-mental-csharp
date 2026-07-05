using System.Windows;
using MentalMathGame.Models;
using MentalMathGame.Models.Enums;
using MentalMathGame.Services;
using MentalMathGame.ViewModels;
using MentalMathGame.Views;

namespace MentalMathGame;

public partial class MainWindow : Window
{
    private readonly SaveService       _saveService   = new();
    private readonly NavigationService _navService    = new();
    private readonly StatisticsService _statsService  = new();
    private PlayerProfile? _currentPlayer;

    public MainWindow()
    {
        InitializeComponent();
        _navService.Initialize(MainContent);
        ShowProfileSelection();
    }

    // ── Navigation principale ─────────────────────────────────────────────

    private void ShowProfileSelection()
    {
        var vm   = new ProfileSelectionViewModel(_saveService, OnProfileSelected);
        var view = new ProfileSelectionView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void OnProfileSelected(PlayerProfile profile)
    {
        _currentPlayer = profile;
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        if (_currentPlayer == null) return;
        var vm   = new MainMenuViewModel(_currentPlayer, ShowGameSetup,
                                         ShowProfileSelection, ShowLeaderboard, ShowStatistics);
        var view = new MainMenuView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    // ── Flux de jeu ──────────────────────────────────────────────────────

    private void ShowGameSetup(GameMode mode)
    {
        if (mode == GameMode.DéfiDuJour)
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            if (_currentPlayer!.LastDailyChallengeDate == today)
            {
                MessageBox.Show(
                    "Tu as déjà relevé le défi du jour !\nReviens demain pour un nouveau défi. 🌟",
                    "Défi du jour", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            StartDailyChallenge();
            return;
        }

        var vm   = new GameSetupViewModel(mode, StartGame, ShowMainMenu);
        var view = new GameSetupView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void StartDailyChallenge()
    {
        int seed = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        var generator = new QuestionGenerator(seed);
        var engine    = new GameEngine(generator);
        engine.StartGame(GameMode.DéfiDuJour, Difficulty.Moyen, _currentPlayer!.Id);

        var vm   = new GameViewModel(engine, GameMode.DéfiDuJour, 10, OnGameEnd, ShowMainMenu);
        var view = new GameView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void StartGame(GameMode mode, Difficulty difficulty, int questionCount)
    {
        var generator = new QuestionGenerator();
        var engine    = new GameEngine(generator);
        engine.StartGame(mode, difficulty, _currentPlayer!.Id);

        var vm   = new GameViewModel(engine, mode, questionCount, OnGameEnd, ShowMainMenu);
        var view = new GameView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void OnGameEnd(GameSession session)
    {
        var score = new Score
        {
            PlayerId   = _currentPlayer!.Id,
            PlayerName = _currentPlayer.Username,
            Points     = session.FinalScore,
            Mode       = session.Mode,
            Difficulty = session.Difficulty,
            Date       = DateTime.Now,
            MaxStreak  = session.MaxStreak
        };
        _saveService.SaveScore(score);

        _currentPlayer.GameHistory.Add(session);
        UpdatePlayerStats(session);

        if (session.Mode == GameMode.DéfiDuJour)
            _currentPlayer.LastDailyChallengeDate = DateTime.Today.ToString("yyyy-MM-dd");

        _saveService.SaveProfile(_currentPlayer);

        var vm = new GameResultViewModel(
            session,
            onPlayAgain:  () => ShowGameSetup(session.Mode),
            onBackToMenu: ShowMainMenu);
        var view = new GameResultView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    // ── Classement et Statistiques ────────────────────────────────────────

    private void ShowLeaderboard()
    {
        if (_currentPlayer == null) return;
        var vm   = new LeaderboardViewModel(_saveService, _currentPlayer.Id, ShowMainMenu);
        var view = new LeaderboardView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void ShowStatistics()
    {
        if (_currentPlayer == null) return;
        var vm   = new StatisticsViewModel(_currentPlayer, _statsService, ShowMainMenu);
        var view = new StatisticsView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    // ── Mise à jour des statistiques du joueur ────────────────────────────

    private void UpdatePlayerStats(GameSession session)
    {
        var stats = _currentPlayer!.Statistics;
        stats.TotalGamesPlayed++;
        stats.TotalPoints += session.FinalScore;

        var modeKey = session.Mode.ToString();
        if (!stats.BestScorePerMode.TryGetValue(modeKey, out int best) || session.FinalScore > best)
            stats.BestScorePerMode[modeKey] = session.FinalScore;

        int totalAnswered = _currentPlayer.GameHistory.Sum(s => s.TotalAnswered);
        int totalCorrect  = _currentPlayer.GameHistory.Sum(s => s.CorrectAnswers);
        stats.GlobalAccuracy = totalAnswered > 0
            ? (double)totalCorrect / totalAnswered * 100
            : 0;

        _statsService.UpdateOperationAccuracy(_currentPlayer);

        string today = DateTime.Today.ToString("yyyy-MM-dd");
        stats.GamesPerDay.TryGetValue(today, out int count);
        stats.GamesPerDay[today] = count + 1;
    }
}
