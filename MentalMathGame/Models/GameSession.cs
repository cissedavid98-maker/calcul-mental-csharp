using MentalMathGame.Models.Enums;

namespace MentalMathGame.Models;

public class GameSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public GameMode Mode { get; set; }
    public Difficulty Difficulty { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int FinalScore { get; set; }
    public int MaxStreak { get; set; }
    public bool IsCompleted { get; set; }
    public List<Question> Questions { get; set; } = [];

    public int CorrectAnswers => Questions.Count(q => q.IsCorrect);
    public int TotalAnswered => Questions.Count(q => q.PlayerAnswer.HasValue);
    public double Accuracy => TotalAnswered == 0 ? 0 : (double)CorrectAnswers / TotalAnswered * 100;
    public TimeSpan Duration => EndTime - StartTime;
}
