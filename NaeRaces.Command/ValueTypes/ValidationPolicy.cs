using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.ValueTypes;

public struct ValidationPolicy
{
    public string Value { get; }
    private ValidationPolicy(string value)
    {
        Value = value;
    }
    public static ValidationPolicy Rehydrate(string value)
    {
        return new ValidationPolicy(value);
    }
    public static ValidationPolicy Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new ValidationPolicy("NONE");

        if (value.Length > 50)
            throw new ArgumentException("Name cannot be longer than 50 characters.", nameof(value));

        return new ValidationPolicy(value.ToUpper());
    }
}