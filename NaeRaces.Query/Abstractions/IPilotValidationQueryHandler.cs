using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IPilotValidationQueryHandler
{
    Task<PilotValidationDetails> GetPilotValidationDetails(Guid pilotId);
}
