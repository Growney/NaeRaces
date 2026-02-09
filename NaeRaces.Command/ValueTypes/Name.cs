using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.ValueTypes;

public struct Name
{
    public string Value { get; }
    private Name(string value)
    {
        Value = value;
    }
    public static Name Rehydrate(string value)
    {
        return new Name(value);
    }
    public static Name Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(value));

        if(value.Length > 100)
            throw new ArgumentException("Name cannot be longer than 100 characters.", nameof(value));

        return new Name(value);
    }
}
