using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.ValueTypes;

public struct Address
{
    public string? AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string? City { get; }
    public string? County { get; }
    public string? Postcode { get; }

    private Address(string? addressLine1, string? addressLine2, string? city, string? county, string? postcode)
    {
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        County = county;
        Postcode = postcode ?? throw new ArgumentNullException(nameof(postcode));
    }

    public static Address Create(string? addressLine1, string? addressLine2, string? city, string? county, string postcode)
    {
        if(addressLine1?.Length > 100)
            throw new ArgumentException("Address line 1 cannot be longer than 100 characters.", nameof(addressLine1));
        if(addressLine2?.Length > 100)
            throw new ArgumentException("Address line 2 cannot be longer than 100 characters.", nameof(addressLine2));
        if(city?.Length > 100)
            throw new ArgumentException("City cannot be longer than 100 characters.", nameof(city));
        if(county?.Length > 100)
            throw new ArgumentException("County cannot be longer than 100 characters.", nameof(county));
        if(postcode?.Length > 10)
            throw new ArgumentException("Postcode cannot be longer than 10 characters.", nameof(postcode));

        if(string.IsNullOrWhiteSpace(postcode))
            throw new ArgumentException("Postcode cannot be null or whitespace.", nameof(postcode));

        return new Address(addressLine1, addressLine2, city, county, postcode);
    }

    public static Address Rehydrate(string? addressLine1, string? addressLine2, string? city, string? county, string? postcode)
    {
        return new Address(addressLine1, addressLine2, city, county, postcode);
    }
}
