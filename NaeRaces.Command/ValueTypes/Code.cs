using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.ValueTypes;

public struct Code
{
    public string Value { get; }

    private Code(string value)
    {
        Value = value;
    }
    public static Code Rehydrate(string value)
    {
        return new Code(value);
    }
    public static Code Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Code cannot be null or whitespace.", nameof(value));

        if(value.Length > 20)
            throw new ArgumentException("Code cannot be longer than 20 characters.", nameof(value));

        foreach(char c in value)
        {
            if (!char.IsLetterOrDigit(c))
                throw new ArgumentException("Code can only contain letters and digits.", nameof(value));
        }
        return new Code(value);
    }
}
