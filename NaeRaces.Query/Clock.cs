using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query;

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
