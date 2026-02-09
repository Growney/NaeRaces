using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events.ValueObjects;

public record Address(string? AddressLine1,string? AddressLine2, string? City, string? Postcode, string? County);
