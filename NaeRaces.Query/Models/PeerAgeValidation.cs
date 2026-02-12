using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PeerAgeValidation(Guid PeerPilotId, Guid PeerPilotClubId, bool IsOnClubCommittee);