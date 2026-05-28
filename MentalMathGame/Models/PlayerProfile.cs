namespace MentalMathGame.Models;

public class PlayerProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public PlayerStatistics Statistics { get; set; } = new();
    public List<Badge> Badges { get; set; } = [];
    public List<GameSession> GameHistory { get; set; } = [];
    public string? LastDailyChallengeDate { get; set; }
}
