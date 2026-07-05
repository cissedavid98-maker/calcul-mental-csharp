namespace MentalMathGame.Models;

public class LeaderboardEntry
{
    public int      Rank            { get; init; }
    public Guid     PlayerId        { get; init; }
    public string   PlayerName      { get; init; } = string.Empty;
    public int      Points          { get; init; }
    public int      MaxStreak       { get; init; }
    public DateTime Date            { get; init; }
    public bool     IsCurrentPlayer { get; init; }

    public string RankDisplay => Rank switch
    {
        1 => "🥇",
        2 => "🥈",
        3 => "🥉",
        _ => $"#{Rank}"
    };

    public string DateDisplay => Date.ToString("dd/MM/yyyy");
}
