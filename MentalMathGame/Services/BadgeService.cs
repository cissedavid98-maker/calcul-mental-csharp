using MentalMathGame.Models;
using MentalMathGame.Models.Enums;

namespace MentalMathGame.Services;

public class BadgeService
{
    public static List<Badge> CreateDefaultBadges() =>
    [
        new Badge
        {
            Id          = "premiere-partie",
            Name        = "Première partie",
            Description = "Jouer sa toute première partie"
        },
        new Badge
        {
            Id          = "sans-faute",
            Name        = "Sans faute",
            Description = "Terminer une partie avec 100 % de précision (min. 5 questions)"
        },
        new Badge
        {
            Id          = "ultra-rapide",
            Name        = "Ultra rapide",
            Description = "Répondre à 20 questions ou plus en mode Chrono"
        },
        new Badge
        {
            Id          = "marathonien",
            Name        = "Marathonien",
            Description = "Jouer 10 parties au total"
        },
        new Badge
        {
            Id          = "maitre-calcul",
            Name        = "Maître du calcul",
            Description = "Atteindre 500 points en une seule partie"
        },
        new Badge
        {
            Id          = "survivant",
            Name        = "Survivant",
            Description = "Terminer une partie Survie sans perdre aucune vie"
        },
        new Badge
        {
            Id          = "expert",
            Name        = "Expert",
            Description = "Obtenir 80 % ou plus en difficulté Difficile (min. 5 questions)"
        }
    ];

    public static void EnsureInitialized(PlayerProfile profile)
    {
        if (profile.Badges.Count == 0)
            profile.Badges.AddRange(CreateDefaultBadges());
    }

    public List<Badge> CheckAndAward(PlayerProfile profile, GameSession session)
    {
        EnsureInitialized(profile);

        var newlyUnlocked = new List<Badge>();
        var stats = profile.Statistics;

        TryUnlock("premiere-partie",
            stats.TotalGamesPlayed >= 1);

        TryUnlock("sans-faute",
            session.Accuracy >= 100 && session.TotalAnswered >= 5);

        TryUnlock("ultra-rapide",
            session.Mode == GameMode.Chrono && session.TotalAnswered >= 20);

        TryUnlock("marathonien",
            stats.TotalGamesPlayed >= 10);

        TryUnlock("maitre-calcul",
            session.FinalScore >= 500);

        TryUnlock("survivant",
            session.Mode == GameMode.Survie &&
            session.Accuracy >= 100 &&
            session.TotalAnswered >= 3);

        TryUnlock("expert",
            session.Difficulty == Difficulty.Difficile &&
            session.Accuracy >= 80 &&
            session.TotalAnswered >= 5);

        return newlyUnlocked;

        void TryUnlock(string id, bool condition)
        {
            var badge = profile.Badges.FirstOrDefault(b => b.Id == id);
            if (badge == null || badge.IsUnlocked || !condition) return;
            badge.IsUnlocked = true;
            badge.UnlockedAt = DateTime.Now;
            newlyUnlocked.Add(badge);
        }
    }
}
