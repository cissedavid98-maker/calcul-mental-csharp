using MentalMathGame.Models;
using MentalMathGame.Models.Enums;

namespace MentalMathGame.Services;

public class StatisticsService
{
    public void UpdateOperationAccuracy(PlayerProfile profile)
    {
        var stats = profile.Statistics;
        foreach (Operation op in Enum.GetValues<Operation>())
        {
            var key = op.ToString();
            var questions = profile.GameHistory
                .SelectMany(s => s.Questions)
                .Where(q => q.Operation == op && q.PlayerAnswer.HasValue)
                .ToList();

            if (questions.Count > 0)
                stats.OperationAccuracy[key] = (double)questions.Count(q => q.IsCorrect) / questions.Count * 100;
        }
    }

    public string GetStrongestOperation(PlayerProfile profile)
    {
        if (profile.Statistics.OperationAccuracy.Count == 0) return "Pas encore de données";
        var op = profile.Statistics.OperationAccuracy.MaxBy(kv => kv.Value).Key;
        return Translate(op);
    }

    public string GetWeakestOperation(PlayerProfile profile)
    {
        if (profile.Statistics.OperationAccuracy.Count == 0) return "Pas encore de données";
        var op = profile.Statistics.OperationAccuracy.MinBy(kv => kv.Value).Key;
        return Translate(op);
    }

    private static string Translate(string op) => op switch
    {
        "Addition"       => "Addition (+)",
        "Soustraction"   => "Soustraction (−)",
        "Multiplication" => "Multiplication (×)",
        "Division"       => "Division (÷)",
        _                => op
    };
}
