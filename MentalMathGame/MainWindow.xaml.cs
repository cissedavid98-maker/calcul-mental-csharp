using System.Windows;
using MentalMathGame.Models;
using MentalMathGame.Models.Enums;
using MentalMathGame.Services;
using MentalMathGame.ViewModels;
using MentalMathGame.Views;

namespace MentalMathGame;

public partial class MainWindow : Window
{
    private readonly SaveService       _saveService = new();
    private readonly NavigationService _navService  = new();
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
        var vm   = new MainMenuViewModel(_currentPlayer, ShowGameSetup, ShowProfileSelection);
        var view = new MainMenuView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    // ── Flux de jeu ──────────────────────────────────────────────────────

    private void ShowGameSetup(GameMode mode)
    {
        var vm   = new GameSetupViewModel(mode, StartGame, ShowMainMenu);
        var view = new GameSetupView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void StartGame(GameMode mode, Difficulty difficulty, int questionCount)
    {
        var generator = new QuestionGenerator();
        var engine    = new GameEngine(generator);
        engine.StartGame(mode, difficulty, _currentPlayer!.Id);

        var vm   = new GameViewModel(engine, mode, questionCount, OnGameEnd);
        var view = new GameView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    private void OnGameEnd(GameSession session)
    {
        // Sauvegarde du score dans le classement
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

        // Mise à jour du profil joueur
        _currentPlayer.GameHistory.Add(session);
        UpdatePlayerStats(session);
        _saveService.SaveProfile(_currentPlayer);

        // Affichage des résultats
        var vm = new GameResultViewModel(
            session,
            onPlayAgain:   () => ShowGameSetup(session.Mode),
            onBackToMenu:  ShowMainMenu);
        var view = new GameResultView { DataContext = vm };
        _navService.NavigateTo(view);
    }

    // ── Mise à jour des statistiques du joueur ────────────────────────────

    private void UpdatePlayerStats(GameSession session)
    {
        var stats = _currentPlayer!.Statistics;
        stats.TotalGamesPlayed++;
        stats.TotalPoints += session.FinalScore;

        // Meilleur score par mode
        var modeKey = session.Mode.ToString();
        if (!stats.BestScorePerMode.TryGetValue(modeKey, out int best) || session.FinalScore > best)
            stats.BestScorePerMode[modeKey] = session.FinalScore;

        // Précision globale recalculée sur tout l'historique
        int totalAnswered = _currentPlayer.GameHistory.Sum(s => s.TotalAnswered);
        int totalCorrect  = _currentPlayer.GameHistory.Sum(s => s.CorrectAnswers);
        stats.GlobalAccuracy = totalAnswered > 0
            ? (double)totalCorrect / totalAnswered * 100
            : 0;

        // Parties par jour
        string today = DateTime.Today.ToString("yyyy-MM-dd");
        stats.GamesPerDay.TryGetValue(today, out int count);
        stats.GamesPerDay[today] = count + 1;
    }
}
