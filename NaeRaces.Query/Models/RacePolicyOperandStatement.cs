using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public class RacePolicyOperandStatement(IRacePolicyStatement Left, string Operator,  IRacePolicyStatement Right, bool IsWithinBrackets) : IRacePolicyStatement
{
    public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime onDate)
    {
        string? leftResult = Left.IsValidForPilot(pilotValidationDetails, onDate);
        string? rightResult = Right.IsValidForPilot(pilotValidationDetails, onDate);

        bool leftValid = string.IsNullOrEmpty(leftResult);
        bool rightValid = string.IsNullOrEmpty(rightResult);

        return Operator switch
        {
            "AND" => leftValid && rightValid ? null : $"({leftResult}) AND ({rightResult})".Trim(' ', 'A', 'N', 'D', '(', ')'),
            "OR" => leftValid || rightValid ? null : $"{leftResult} OR {rightResult}".Trim(' ', 'O', 'R', '(', ')'),
            _ => throw new InvalidOperationException($"Unsupported operator: {Operator}")
        };
    }
}
