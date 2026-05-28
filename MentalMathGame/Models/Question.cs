using MentalMathGame.Models.Enums;

namespace MentalMathGame.Models;

public class Question
{
    public int Operand1 { get; set; }
    public int Operand2 { get; set; }
    public Operation Operation { get; set; }
    public int CorrectAnswer { get; set; }
    public int? PlayerAnswer { get; set; }
    public bool IsCorrect => PlayerAnswer.HasValue && PlayerAnswer.Value == CorrectAnswer;
    public TimeSpan ResponseTime { get; set; }

    public string Display => Operation switch
    {
        Operation.Addition       => $"{Operand1} + {Operand2}",
        Operation.Soustraction   => $"{Operand1} - {Operand2}",
        Operation.Multiplication => $"{Operand1} × {Operand2}",
        Operation.Division       => $"{Operand1} ÷ {Operand2}",
        _                        => string.Empty
    };
}
