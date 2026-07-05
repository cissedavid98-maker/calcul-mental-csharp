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
    private readonly Action _onAbandon;

    private Question? _currentQuestion;
    private string _answerText      = string.Empty;
    private int    _score;
    private int    _streak;
    private int    _timeLeft        = 60;
    private string _feedbackMessage = string.Empty;
    private bool   _showFeedback;
    private bool   _feedbackIsCorrect;
    private bool   _inputEnabled    = true;
    private int    _questionNumber;
    private bool   _gameOver;
    private int    _lives           = 3;

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
        set
        {
            Set(ref _questionNumber, value);
            OnPropertyChanged(nameof(QuestionCounterText));
        }
    }

    public int Lives
    {
        get => _lives;
        private set
        {
            Set(ref _lives, value);
            OnPropertyChanged(nameof(LivesDisplay));
        }
    }

    public string LivesDisplay =>
        string.Concat(Enumerable.Repeat("❤️ ", _lives)) +
        string.Concat(Enumerable.Repeat("🖤 ", 3 - _lives));

    public int    TotalQuestions      => _totalQuestions;
    public string QuestionCounterText => $"Question {_questionNumber} / {_totalQuestions}";
    public bool   IsChronoMode        => _mode == GameMode.Chrono;
    public bool   IsSerieMode         => _mode == GameMode.Serie;
    public bool   IsSurvieMode        => _mode == GameMode.Survie;
    public bool   ShowQuestionCounter => _mode == GameMode.Serie || _mode == GameMode.DéfiDuJour;

    public RelayCommand SubmitCommand  { get; }
    public RelayCommand AbandonCommand { get; }

    // ── Constructeur ──────────────────────────────────────────────────────

    public GameViewModel(GameEngine engine, GameMode mode, int totalQuestions, Action<GameSession> onGameEnd, Action onAbandon)
    {
        _engine         = engine;
        _mode           = mode;
        _totalQuestions = totalQuestions;
        _onGameEnd      = onGameEnd;
        _onAbandon      = onAbandon;

        SubmitCommand  = new RelayCommand(Submit, () => InputEnabled && int.TryParse(AnswerText, out _));
        AbandonCommand = new RelayCommand(Abandon);

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

        bool hasLimit = _mode == GameMode.Serie || _mode == GameMode.DéfiDuJour;
        if (hasLimit && _questionNumber >= _totalQuestions)
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

        bool gameOverAfterFeedback = false;
        if (!result.IsCorrect && _mode == GameMode.Survie)
        {
            Lives--;
            if (_lives <= 0) gameOverAfterFeedback = true;
        }

        FeedbackMessage = BuildFeedbackMessage(result, gameOverAfterFeedback);
        ShowFeedback = true;

        _feedbackTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1100) };
        _feedbackTimer.Tick += (_, _) =>
        {
            _feedbackTimer.Stop();
            if (gameOverAfterFeedback) FinishGame(); else LoadNextQuestion();
        };
        _feedbackTimer.Start();
    }

    private string BuildFeedbackMessage(AnswerResult result, bool gameOver)
    {
        if (result.IsCorrect)
            return result.CurrentStreak > 3
                ? $"✅  Bonne réponse !  +{result.PointsEarned} pts  🔥  Série de {result.CurrentStreak} !"
                : $"✅  Bonne réponse !  +{result.PointsEarned} pts";

        return gameOver
            ? $"❌  La réponse était  {result.CorrectAnswer}  —  Plus de vies !"
            : $"❌  La réponse était  {result.CorrectAnswer}";
    }

    private void Abandon()
    {
        if (_gameOver) return;
        _gameOver = true;
        _chronoTimer?.Stop();
        _feedbackTimer?.Stop();
        _onAbandon();
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
