using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IClubDetailsQueryHandler
{
    Task<bool> DoesClubExist(Guid clubId);
}
