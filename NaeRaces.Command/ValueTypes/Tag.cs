using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.ValueTypes;

public struct Tag
{
    public string Value { get;}

    private Tag(string value)
    {
        Value = value;
    }

    public static Tag Create(string value) 
    { 
        if (string.IsNullOrWhiteSpace(value)) 
            throw new ArgumentException("Tag value cannot be null or whitespace.", nameof(value));
        
        if(value.Length > 15)
        {
            throw new ArgumentException("Tag value cannot exceed 15 characters.", nameof(value));
        }
        
        return new Tag(value); 
    }

    public static Tag Rehydrate(string value) => new (value);

}
