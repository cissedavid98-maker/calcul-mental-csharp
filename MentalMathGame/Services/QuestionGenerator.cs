using MentalMathGame.Models;
using MentalMathGame.Models.Enums;

namespace MentalMathGame.Services;

public class QuestionGenerator
{
    private readonly Random _random;

    public QuestionGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public Question Generate(Difficulty difficulty, Operation? forced = null)
    {
        var op = forced ?? (Operation)_random.Next(4);
        return op switch
        {
            Operation.Addition       => BuildAddition(difficulty),
            Operation.Soustraction   => BuildSoustraction(difficulty),
            Operation.Multiplication => BuildMultiplication(difficulty),
            Operation.Division       => BuildDivision(difficulty),
            _                        => BuildAddition(difficulty)
        };
    }

    private (int min, int max) BaseRange(Difficulty d) => d switch
    {
        Difficulty.Facile    => (1, 20),
        Difficulty.Moyen     => (10, 100),
        Difficulty.Difficile => (50, 999),
        _                    => (1, 20)
    };

    // Plage réduite pour ×/÷ afin d'éviter des résultats ingérables
    private (int min, int max) MultiRange(Difficulty d) => d switch
    {
        Difficulty.Facile    => (1, 10),
        Difficulty.Moyen     => (2, 20),
        Difficulty.Difficile => (2, 30),
        _                    => (1, 10)
    };

    private int Rand(int min, int max) => _random.Next(min, max + 1);

    private Question BuildAddition(Difficulty d)
    {
        var (min, max) = BaseRange(d);
        int a = Rand(min, max), b = Rand(min, max);
        return Make(a, b, Operation.Addition, a + b);
    }

    private Question BuildSoustraction(Difficulty d)
    {
        var (min, max) = BaseRange(d);
        int a = Rand(min, max), b = Rand(min, max);
        // Mode Facile : résultat toujours positif
        if (d == Difficulty.Facile && b > a) (a, b) = (b, a);
        return Make(a, b, Operation.Soustraction, a - b);
    }

    private Question BuildMultiplication(Difficulty d)
    {
        var (min, max) = MultiRange(d);
        int a = Rand(min, max), b = Rand(min, max);
        return Make(a, b, Operation.Multiplication, a * b);
    }

    private Question BuildDivision(Difficulty d)
    {
        var (min, max) = MultiRange(d);
        int divisor  = Rand(Math.Max(min, 2), max);
        int quotient = Rand(1, max);
        int dividend = divisor * quotient;   // division toujours exacte
        return Make(dividend, divisor, Operation.Division, quotient);
    }

    private static Question Make(int a, int b, Operation op, int answer) => new()
    {
        Operand1      = a,
        Operand2      = b,
        Operation     = op,
        CorrectAnswer = answer
    };
}
