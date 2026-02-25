using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}
