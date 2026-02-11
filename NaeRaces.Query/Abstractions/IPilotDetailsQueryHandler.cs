using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IPilotDetailsQueryHandler
{
    Task<bool> DoesPilotExist(Guid pilotId);
}
