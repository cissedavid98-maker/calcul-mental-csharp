using MentalMathGame.Models.Enums;

namespace MentalMathGame.ViewModels;

public class GameSetupViewModel : BaseViewModel
{
    private readonly GameMode _mode;
    private readonly Action<GameMode, Difficulty, int> _onStart;
    private readonly Action _onBack;

    private Difficulty? _selectedDifficulty;
    private int _questionCount = 10;

    public string ModeTitle => _mode switch
    {
        GameMode.Chrono    => "⏱️  Mode Chrono",
        GameMode.Serie     => "📋  Mode Série",
        GameMode.Survie    => "❤️  Mode Survie",
        GameMode.DéfiDuJour => "🌟  Défi du jour",
        _                  => string.Empty
    };

    public string ModeDescription => _mode switch
    {
        GameMode.Chrono    => "Réponds à un maximum de questions en 60 secondes.",
        GameMode.Serie     => "Réponds à une série de questions sans contrainte de temps.",
        GameMode.Survie    => "Tu disposes de 3 vies — chaque erreur en retire une.",
        GameMode.DéfiDuJour => "Un défi unique généré chaque jour, jouable une seule fois.",
        _                  => string.Empty
    };

    public bool IsSerieMode => _mode == GameMode.Serie;

    public Difficulty? SelectedDifficulty
    {
        get => _selectedDifficulty;
        set
        {
            Set(ref _selectedDifficulty, value);
            OnPropertyChanged(nameof(IsFacileSelected));
            OnPropertyChanged(nameof(IsMoyenSelected));
            OnPropertyChanged(nameof(IsDifficileSelected));
            OnPropertyChanged(nameof(CanStart));
        }
    }

    public int QuestionCount
    {
        get => _questionCount;
        set
        {
            Set(ref _questionCount, value);
            OnPropertyChanged(nameof(Is10Selected));
            OnPropertyChanged(nameof(Is20Selected));
            OnPropertyChanged(nameof(Is30Selected));
        }
    }

    public bool IsFacileSelected   => _selectedDifficulty == Difficulty.Facile;
    public bool IsMoyenSelected    => _selectedDifficulty == Difficulty.Moyen;
    public bool IsDifficileSelected => _selectedDifficulty == Difficulty.Difficile;
    public bool Is10Selected       => _questionCount == 10;
    public bool Is20Selected       => _questionCount == 20;
    public bool Is30Selected       => _questionCount == 30;
    public bool CanStart           => _selectedDifficulty.HasValue;

    public RelayCommand<string> SelectDifficultyCommand  { get; }
    public RelayCommand<string> SelectQuestionCountCommand { get; }
    public RelayCommand StartCommand { get; }
    public RelayCommand BackCommand  { get; }

    public GameSetupViewModel(GameMode mode, Action<GameMode, Difficulty, int> onStart, Action onBack)
    {
        _mode    = mode;
        _onStart = onStart;
        _onBack  = onBack;

        SelectDifficultyCommand = new RelayCommand<string>(d =>
        {
            if (Enum.TryParse<Difficulty>(d, out var diff)) SelectedDifficulty = diff;
        });

        SelectQuestionCountCommand = new RelayCommand<string>(c =>
        {
            if (int.TryParse(c, out var n)) QuestionCount = n;
        });

        StartCommand = new RelayCommand(
            () => _onStart(_mode, _selectedDifficulty!.Value, _questionCount),
            () => CanStart);

        BackCommand = new RelayCommand(_onBack);
    }
}
