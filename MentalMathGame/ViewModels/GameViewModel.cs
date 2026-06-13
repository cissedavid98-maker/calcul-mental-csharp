using System.Windows.Threading;
using MentalMathGame.Models;
using MentalMathGame.Models.Enums;
using MentalMathGame.Services;

namespace MentalMathGame.ViewModels;

public class GameViewModel : BaseViewModel
{
    private readonly GameEngine _engine;
    private readonly GameMode   _mode;
    private readonly int        _totalQuestions;
    private readonly Action<GameSession> _onGameEnd;

    private Question? _currentQuestion;
    private string _answerText    = string.Empty;
    private int    _score;
    private int    _streak;
    private int    _timeLeft      = 60;
    private string _feedbackMessage = string.Empty;
    private bool   _showFeedback;
    private bool   _feedbackIsCorrect;
    private bool   _inputEnabled  = true;
    private int    _questionNumber;
    private bool   _gameOver;

    private DispatcherTimer? _chronoTimer;
    private DispatcherTimer? _feedbackTimer;

    // ── Propriétés exposées à la Vue ──────────────────────────────────────

    public string ModeLabel => _mode switch
    {
        GameMode.Chrono     => "⏱️  CHRONO",
        GameMode.Serie      => "📋  SÉRIE",
        GameMode.Survie     => "❤️  SURVIE",
        GameMode.DéfiDuJour => "🌟  DÉFI DU JOUR",
        _                   => string.Empty
    };

    public Question? CurrentQuestion
    {
        get => _currentQuestion;
        set => Set(ref _currentQuestion, value);
    }

    public string AnswerText
    {
        get => _answerText;
        set => Set(ref _answerText, value);
    }

    public int Score
    {
        get => _score;
        set => Set(ref _score, value);
    }

    public int Streak
    {
        get => _streak;
        set => Set(ref _streak, value);
    }

    public int TimeLeft
    {
        get => _timeLeft;
        set => Set(ref _timeLeft, value);
    }

    public string FeedbackMessage
    {
        get => _feedbackMessage;
        set => Set(ref _feedbackMessage, value);
    }

    public bool ShowFeedback
    {
        get => _showFeedback;
        set => Set(ref _showFeedback, value);
    }

    public bool FeedbackIsCorrect
    {
        get => _feedbackIsCorrect;
        set => Set(ref _feedbackIsCorrect, value);
    }

    public bool InputEnabled
    {
        get => _inputEnabled;
        set => Set(ref _inputEnabled, value);
    }

    public int QuestionNumber
    {
        get => _questionNumber;
        set => Set(ref _questionNumber, value);
    }

    public int TotalQuestions => _totalQuestions;
    public bool IsChronoMode  => _mode == GameMode.Chrono;
    public bool IsSerieMode   => _mode == GameMode.Serie;

    public RelayCommand SubmitCommand { get; }

    // ── Constructeur ──────────────────────────────────────────────────────

    public GameViewModel(GameEngine engine, GameMode mode, int totalQuestions, Action<GameSession> onGameEnd)
    {
        _engine         = engine;
        _mode           = mode;
        _totalQuestions = totalQuestions;
        _onGameEnd      = onGameEnd;

        SubmitCommand = new RelayCommand(Submit, () => InputEnabled && int.TryParse(AnswerText, out _));

        if (mode == GameMode.Chrono) StartChronoTimer();

        LoadNextQuestion();
    }

    // ── Logique interne ───────────────────────────────────────────────────

    private void StartChronoTimer()
    {
        _chronoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _chronoTimer.Tick += (_, _) =>
        {
            TimeLeft--;
            if (TimeLeft <= 0) { _chronoTimer.Stop(); FinishGame(); }
        };
        _chronoTimer.Start();
    }

    private void LoadNextQuestion()
    {
        if (_gameOver) return;

        if (_mode == GameMode.Serie && _questionNumber >= _totalQuestions)
        {
            FinishGame();
            return;
        }

        CurrentQuestion = _engine.NextQuestion();
        QuestionNumber++;
        AnswerText   = string.Empty;
        ShowFeedback = false;
        InputEnabled = true;
    }

    private void Submit()
    {
        if (CurrentQuestion == null || !int.TryParse(AnswerText, out int answer)) return;

        InputEnabled = false;
        var result = _engine.SubmitAnswer(CurrentQuestion, answer);

        Score  = result.CurrentScore;
        Streak = result.CurrentStreak;
        FeedbackIsCorrect = result.IsCorrect;

        FeedbackMessage = result.IsCorrect
            ? (result.CurrentStreak > 3
                ? $"✅  Bonne réponse !  +{result.PointsEarned} pts  🔥  Série de {result.CurrentStreak} !"
                : $"✅  Bonne réponse !  +{result.PointsEarned} pts")
            : $"❌  La réponse était  {result.CorrectAnswer}";

        ShowFeedback = true;

        // Passage automatique à la question suivante après 1,1 seconde
        _feedbackTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1100) };
        _feedbackTimer.Tick += (_, _) => { _feedbackTimer.Stop(); LoadNextQuestion(); };
        _feedbackTimer.Start();
    }

    private void FinishGame()
    {
        if (_gameOver) return;
        _gameOver = true;

        _chronoTimer?.Stop();
        _feedbackTimer?.Stop();
        InputEnabled = false;

        _onGameEnd(_engine.EndGame());
    }
}
