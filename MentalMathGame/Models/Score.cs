using MentalMathGame.Models.Enums;

namespace MentalMathGame.Models;

public class Score
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Points { get; set; }
    public GameMode Mode { get; set; }
    public Difficulty Difficulty { get; set; }
    public DateTime Date { get; set; }
    public int MaxStreak { get; set; }
}
