using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.ValueTypes;


public struct CallSign
{
    public string Value { get; }

    private CallSign(string value)
    {
        Value = value;
    }
    public static CallSign Rehydrate(string value)
    {
        return new CallSign(value);
    }
    public static CallSign Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Callsign cannot be null or whitespace.", nameof(value));

        if (value.Length > 25)
            throw new ArgumentException("Callsign cannot be longer than 25 characters.", nameof(value));

        if (value.Length < 3)
            throw new ArgumentException("Callsign cannot be less than 3 characters");

        return new CallSign(value);
    }
}
