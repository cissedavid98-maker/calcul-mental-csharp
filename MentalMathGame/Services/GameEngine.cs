using MentalMathGame.Models;
using MentalMathGame.Models.Enums;

namespace MentalMathGame.Services;

public class GameEngine
{
    private readonly QuestionGenerator _generator;
    private GameSession? _session;
    private int _currentStreak;
    private int _score;
    private DateTime _questionStartTime;

    public int CurrentStreak => _currentStreak;
    public int CurrentScore  => _score;

    public GameEngine(QuestionGenerator generator)
    {
        _generator = generator;
    }

    public void StartGame(GameMode mode, Difficulty difficulty, Guid playerId)
    {
        _session = new GameSession
        {
            PlayerId   = playerId,
            Mode       = mode,
            Difficulty = difficulty,
            StartTime  = DateTime.Now
        };
        _currentStreak   = 0;
        _score           = 0;
        _questionStartTime = DateTime.Now;
    }

    public Question NextQuestion()
    {
        _questionStartTime = DateTime.Now;
        return _generator.Generate(_session!.Difficulty);
    }

    public AnswerResult SubmitAnswer(Question question, int playerAnswer)
    {
        question.PlayerAnswer  = playerAnswer;
        question.ResponseTime  = DateTime.Now - _questionStartTime;
        _session!.Questions.Add(question);

        bool isCorrect = question.IsCorrect;
        int  points    = 0;

        if (isCorrect)
        {
            _currentStreak++;
            points  = CalculatePoints(_currentStreak);
            _score += points;
        }
        else
        {
            _currentStreak = 0;
        }

        _session.FinalScore = _score;
        _session.MaxStreak  = Math.Max(_session.MaxStreak, _currentStreak);

        return new AnswerResult
        {
            IsCorrect      = isCorrect,
            CorrectAnswer  = question.CorrectAnswer,
            PointsEarned   = points,
            CurrentScore   = _score,
            CurrentStreak  = _currentStreak
        };
    }

    public GameSession EndGame()
    {
        _session!.EndTime     = DateTime.Now;
        _session.IsCompleted  = true;
        _session.FinalScore   = _score;
        return _session;
    }

    // Barème : plus la série est longue, plus chaque bonne réponse rapporte
    private static int CalculatePoints(int streak) => streak switch
    {
        <= 3 => 10,
        <= 6 => 15,
        <= 9 => 20,
        _    => 30
    };
}
