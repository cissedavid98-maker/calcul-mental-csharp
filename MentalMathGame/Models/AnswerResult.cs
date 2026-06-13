namespace MentalMathGame.Models;

public class AnswerResult
{
    public bool IsCorrect { get; set; }
    public int CorrectAnswer { get; set; }
    public int PointsEarned { get; set; }
    public int CurrentScore { get; set; }
    public int CurrentStreak { get; set; }
}
