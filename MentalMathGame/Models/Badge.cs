namespace MentalMathGame.Models;

public class Badge
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
}
