using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IPeerValidation
{
    public Guid PeerPilotId { get; }
    public Guid PeerPilotClubId { get; }
    public bool IsOnClubCommittee { get; }
}
